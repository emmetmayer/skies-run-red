using System.Collections;
using System.Collections.Generic;
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
    ControlLocked, //use this when you dont want the player to move
    DoubleJumped,
    
    //i can do like. sliding/double-jumped but im not sure
    //might need to do this for multi step abilities/basic combos
}

public enum PlayerState
{
    NONE,
    Grounded,
    Airborne,
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
    public PlayerState _pState;

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
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            _cmRef.gameObject.SetActive(true);
            _cm3Ref = _cmRef.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            _piRef.enabled = true;
            _ccRef = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        _pState = _ccRef.isGrounded ? PlayerState.Grounded : PlayerState.Airborne;

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
                //Debug.Log(Pointer.current.delta);
                //Debug.Log(Pointer.current.delta.ReadValue());
               
                //Debug.Log(Pointer.current.position);
                //Debug.Log(Pointer.current.position.ReadValue());
                
                //Debug.Log(Pointer.current.pressure);
                //Debug.Log(Pointer.current.pressure.ReadValue());
                
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
        //Debug.Log(dir);
        dir = new Vector3(dir.x, 0, dir.z);
        dir = dir.normalized;
        dir *= Speed;
        
        //this should be couched into a grounded check probably
        //add dashing/dodge check to stall reset vert vel to 0
        if (_verticalVelocity > 0 || !_ccRef.isGrounded)
        {
            _verticalVelocity -= Gravity;
            //add a max 
        }
        else
        {
            _verticalVelocity = -_ccRef.stepOffset;
        }
        //Debug.Log(_verticalVelocity);
        dir.y = _verticalVelocity;
        //Debug.Log(dir);
        _ccRef.Move(Time.fixedDeltaTime * dir);
        //_rbRef.velocity =  Speed * dir;
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
        if (_pState == PlayerState.Grounded) _cState = ControlState.NONE;
        if (_cState == ControlState.DoubleJumped || _cState == ControlState.ControlLocked)
        {
            return;
        }
        _verticalVelocity = JumpForce;
        if (_pState == PlayerState.Airborne) _cState = ControlState.DoubleJumped;
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
        if (Time.time - _lastDash < DashCooldown)
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
        float movePerTick =  DashDistance / (DashDuration/ _movementTimestep);
        movePerTick = Mathf.Min(DashDistance, movePerTick);
       
        for (int i = 0; i < DashDuration / _movementTimestep; i++)
        {
            //Debug.Log("movepertick " + movePerTick + " and in dir " + dir);
            _ccRef.Move(Time.fixedDeltaTime * Speed * movePerTick * dir);
            yield return new WaitForSeconds(_movementTimestep);
        }

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
        if (Time.time - _lastDodge < DodgeCooldown)
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
        float movePerTick =  DodgeDistance / (DodgeDuration/ _movementTimestep);
        movePerTick = Mathf.Min(DodgeDistance, movePerTick);
       
        for (int i = 0; i < DodgeDuration / _movementTimestep; i++)
        {
            //Debug.Log("movepertick " + movePerTick + " and in dir " + dir);
            _ccRef.Move(Time.fixedDeltaTime * Speed * movePerTick * dir);
            yield return new WaitForSeconds(_movementTimestep);
        }

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
