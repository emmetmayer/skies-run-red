using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;
using Unity.Netcode;
using Cinemachine;
using Unity.Netcode.Components;

//might be better to just do enums for super broad states/contexts, and then do bools for other allowances
public enum ControlState
{
    NONE,
    
    //i can do like. sliding/double-jumped but im not sure
    //might need to do this for multi step abilities/basic combos
}

public enum PlayerState
{
    NONE,
    Grounded,
    Airborne,
    ControlLocked, //use this when you dont want the player to move
    DoubleJumped,
    Dashed,
    Dodged,
    
}
public enum CardinalDirections
{
    Forward,
    Backward,
    Left,
    Right
}
public class PlayerMovement : NetworkBehaviour
{
    private ControlState _cState;
    public HashSet<PlayerState> _pState = new HashSet<PlayerState>();

    [Header("References to relevant components")]
    [SerializeField] private CharacterController _ccRef;
    [SerializeField] private CinemachineVirtualCamera _cmRef;
    private Cinemachine3rdPersonFollow _cm3Ref;
    [SerializeField] private PlayerInput _piRef;
    [Header("Animation Refs")]
    [SerializeField] private NetworkAnimator _naRef;
    [SerializeField] private AnimationClip _attack;


    [Header("Movement Specs")]
    public float Gravity;
    public float MouseSense = 5f;
    public float Speed = 5f;
    public float JumpForce;
    public float TurnSmoothVelocity;
    [Range(-4f, 4f)] public float VerticalLook = 0f;

    [SerializeField] private float _mouseXFactor = 4f;
    [SerializeField] private float _mouseYFactor = .04f;

    [Header("Dash Traits")]
    public float DashDistance;
    public float DashCooldown;
    public float DashDuration;
    private float _lastDash;
    
    [Header("Dodge Traits")]
    public float DodgeDistance;
    public float DodgeCooldown;
    public float DodgeDuration;
    private float _lastDodge;
    private float _verticalVelocity = 0f;

    private float _movementTimestep = .005f;

    private void Awake()
    {
        _ccRef = GetComponent<CharacterController>();
        if (_ccRef != null)
        {
            // CharacterController must start disabled for spawn position to work
            _ccRef.enabled = false;
        }
    }
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            // Owner enables character controller when spawned
            if (_ccRef != null)
            {
                _ccRef.enabled = true;
            }
            _cmRef.gameObject.SetActive(true);
            _cm3Ref = _cmRef.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            _piRef.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        base.OnNetworkSpawn();
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        //_pState = _ccRef.isGrounded ? PlayerState.Grounded : PlayerState.Airborne;
        if (_ccRef.isGrounded)
        {
            _pState.Add(PlayerState.Grounded);
            if(_pState.Contains(PlayerState.Airborne)) _pState.Remove(PlayerState.Airborne);
        }
        else
        {
            if (_pState.Contains(PlayerState.Grounded)) _pState.Remove(PlayerState.Grounded);
            _pState.Add(PlayerState.Airborne);
        }
        var delta = Mouse.current.delta.ReadValue();

        var move = _piRef.actions["Move"].ReadValue<Vector2>();

        if (/*IsServer && */IsLocalPlayer)
        {
            HandleLook(delta);
            HandleVertLook(delta);
            //Debug.LogError(_pState);
            //Debug.LogError(_verticalVelocity);
            HandleMove(move);
        }
        /*else if (IsLocalPlayer)
        {
            HandleLookServerRpc(delta);
            HandleVertLook(delta);
            //Debug.LogError(_pState);
            //Debug.LogError(_verticalVelocity);
            HandleMoveServerRpc(move);
        }*/
        //any external mods
        
        //send it to server
        
        
        //_dir = transform.forward;
        //transform.position = new Vector3(0, 0, 0);
    }
    
    
#region Input

    private void HandleLook(Vector2 delta)
    {
        
        if (Keyboard.current.enabled)
        {
            if (!Mouse.current.enabled) {

                InputSystem.EnableDevice(Mouse.current);

            }
            if (Mouse.current.enabled)
            {
                //Debug.Log("Mouse works");
            }
            if (Pointer.current.enabled)
            {

            }
            
            //Debug.Log(delta);
            if (delta.x == 0)
            {
                return;
            }
            
            //delta *= 0.5f; // Account for scaling applied directly in Windows code by old input system.
            //delta *= 0.1f; // Account for sensitivity setting on old Mouse X and Y axes.

            int dir = delta.x > 0 ? 1 : -1;

           
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, transform.eulerAngles.y + (dir * MouseSense * _mouseXFactor * Time.fixedDeltaTime), ref TurnSmoothVelocity, 0f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

        }

    }

    private void HandleVertLook(Vector2 delta)
    {
        if (delta.y == 0)
        {
            return;
        }
        VerticalLook -= delta.y * MouseSense * _mouseYFactor * Time.fixedDeltaTime;
        VerticalLook = Mathf.Clamp(VerticalLook, -4f, 4f);
        _cm3Ref.VerticalArmLength = VerticalLook;
    }


    //it would be fine to just . call this in update actually i will do that
    private void HandleMove(Vector2 move)
    {
        //this currently only knows about keyboard
        
        var dir = Vector3.zero;

        dir += (transform.right * move.x).normalized;
        dir += (transform.forward * move.y).normalized;
        dir = new Vector3(dir.x, 0, dir.z);
        dir = dir.normalized;
        dir *= Speed;
        
        //this should be couched into a grounded check probably
        //add dashing/dodge check to stall reset vert vel to 0
        
        //convert this 
        if ((_verticalVelocity > 0 || !_ccRef.isGrounded) && (!_pState.Contains(PlayerState.Dashed) || _pState.Contains(PlayerState.Dodged)))
        {
            _verticalVelocity -= Gravity;
            //add a max 
        }
        else
        {
            _verticalVelocity = -_ccRef.stepOffset;
        }
        dir.y = _verticalVelocity;
        _ccRef.Move(Time.fixedDeltaTime * dir);
    }

    [ServerRpc]
    private void HandleLookServerRpc(Vector2 delta)
    {
        HandleLook(delta);
    }

    [ServerRpc]
    private void HandleMoveServerRpc(Vector2 move)
    {
        HandleMove(move);
    }

    public void OnJump()
    {
        if (/*IsServer && */IsLocalPlayer)
        {
            Jump();
        }
        /*else if (IsLocalPlayer)
        {
            JumpServerRpc();
        }*/
        
    }

    private void Jump()
    {
        //set y velocity
        if (_pState.Contains(PlayerState.Grounded))
        {
            _pState.Remove(PlayerState.DoubleJumped);
        }
        if (_pState.Contains(PlayerState.DoubleJumped))
        {
            return;
        }
        _verticalVelocity = JumpForce;
        if (_pState.Contains(PlayerState.Airborne)) _pState.Add(PlayerState.DoubleJumped);
    }

    [ServerRpc]
    private void JumpServerRpc()
    {
        Jump();
    }
    
    public void OnSlide()
    {
        
    }
    public void OnCrouch()
    {
        
    }
    public void OnForwardDash()
    {
        //lock input? do fast movement
        OnDash(CardinalDirections.Forward);
    }
    public void OnBackwardDash()
    {
        //lock input? do fast movement
        OnDash(CardinalDirections.Backward);
    }
    public void OnLeftDash()
    {
        //lock input? do fast movement
        OnDash(CardinalDirections.Left);
    }
    public void OnRightDash()
    {
        //lock input? do fast movement
        OnDash(CardinalDirections.Right);
    }

    private void OnDash(CardinalDirections dir)
    {
        if (/*IsServer && */IsLocalPlayer)
        {
            Dash(dir);
        }
        /*else if (IsLocalPlayer)
        {
            DashServerRpc(dir);
        }*/
    }

    private void Dash(CardinalDirections dir)
    {
        if (Time.time - _lastDash < DashCooldown || 
            _pState.Contains(PlayerState.Dashed) || 
            _pState.Contains(PlayerState.Dodged))
        {
            return;
        }

        _lastDash = Time.time;
        //add cd check
        Vector3 moveDir;
        switch (dir)
        {
            case CardinalDirections.Forward:
                moveDir = transform.forward;
                break;
            case CardinalDirections.Backward:
                moveDir = -transform.forward;
                break;
            case CardinalDirections.Left:
                moveDir = -transform.right;
                break;
            case CardinalDirections.Right:
                moveDir = transform.right;
                break;
            default:
                moveDir = Vector3.zero;
                break;
        }

        StartCoroutine(DashOverTime(moveDir));
        //_ccRef.Move(Time.fixedDeltaTime* moveDir * Speed * DashDistance);
    }
    
    IEnumerator DashOverTime(Vector3 dir)
    {
        _pState.Add(PlayerState.Dashed);
        float movePerTick =  DashDistance / (DashDuration/ _movementTimestep);
        movePerTick = Mathf.Min(DashDistance, movePerTick);
       
        for (int i = 0; i < DashDuration / _movementTimestep; i++)
        {
            //Debug.Log("movepertick " + movePerTick + " and in dir " + dir);
            _ccRef.Move(Time.fixedDeltaTime * Speed * movePerTick * dir);
            yield return new WaitForSeconds(_movementTimestep);
        }

        _pState.Remove(PlayerState.Dashed);
        yield break;
    }

    [ServerRpc]
    private void DashServerRpc(CardinalDirections dir)
    {
        Dash(dir);
    }

    public void OnForwardDodge()
    {
        OnDodge(CardinalDirections.Forward);
    }
    public void OnBackwardDodge()
    {
        OnDodge(CardinalDirections.Backward);
    }
    public void OnLeftDodge()
    {
        OnDodge(CardinalDirections.Left);
    }
    public void OnRightDodge()
    {
        OnDodge(CardinalDirections.Right);
    }

    private void OnDodge(CardinalDirections dir)
    {
        if (/*IsServer && */IsLocalPlayer)
        {
            Dodge(dir);
        }
        /*else if (IsLocalPlayer)
        {
            DodgeServerRpc(dir);
        }*/
    }

    private void Dodge(CardinalDirections dir)
    {
        if (Time.time - _lastDodge < DodgeCooldown|| 
            _pState.Contains(PlayerState.Dashed) || 
            _pState.Contains(PlayerState.Dodged))
        {
            return;
        }

        _lastDodge = Time.time;
        
        //add cd check
        Debug.Log("Dodging");
        //lock input? do short, fast movement
        Vector3 moveDir;
        switch (dir)
        {
            case CardinalDirections.Forward:
                moveDir = transform.forward;
                break;
            case CardinalDirections.Backward:
                moveDir = -transform.forward;
                break;
            case CardinalDirections.Left:
                moveDir = -transform.right;
                break;
            case CardinalDirections.Right:
                moveDir = transform.right;
                break;
            default:
                moveDir = Vector3.zero;
                break;
        }

        StartCoroutine(DodgeOverTime(moveDir));
        //_ccRef.Move(Time.fixedDeltaTime* moveDir * Speed * DodgeDistance); 
    }
    
    IEnumerator DodgeOverTime(Vector3 dir)
    {
        _pState.Add(PlayerState.Dodged);
        float movePerTick =  DodgeDistance / (DodgeDuration/ _movementTimestep);
        movePerTick = Mathf.Min(DodgeDistance, movePerTick);
       
        for (int i = 0; i < DodgeDuration / _movementTimestep; i++)
        {
            //Debug.Log("movepertick " + movePerTick + " and in dir " + dir);
            _ccRef.Move(Time.fixedDeltaTime * Speed * movePerTick * dir);
            yield return new WaitForSeconds(_movementTimestep);
        }

        _pState.Remove(PlayerState.Dodged);
        yield break;
    }

    [ServerRpc]
    private void DodgeServerRpc(CardinalDirections dir)
    {
        Dodge(dir);
    }

    public void OnFire()
    {
        if (/*IsServer && */IsLocalPlayer)
        {
            Fire();
        }
        /*else if (IsLocalPlayer)
        {
            FireServerRpc();
        }*/
        
    }

    private void Fire()
    {
        //_naRef.Animator.Play(_attack);
        _naRef.Animator.Play("SwingReal");
        //_atkAnim.Play();
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        Fire();
    }

    public void OnParry()
    {
        
    }
    public void OnAbility1()
    {
        
    }
    public void OnAbility2()
    {
        
    }
    public void OnAbility3()
    {
        
    }
    public void OnAbility4()
    {
        
    }

    #endregion
}
