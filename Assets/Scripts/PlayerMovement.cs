using System;
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
    //private ControlState _cState;
    public HashSet<PlayerState> _pState = new HashSet<PlayerState>();
    
    [Header("References to relevant components")]
    [SerializeField] private CharacterController _ccRef;
    [SerializeField] private GameObject _cbRef;
    [SerializeField] private GameObject _cfRef;
    [SerializeField] private CinemachineVirtualCamera _cmRef;
    [SerializeField] private PlayerInput _piRef;
    [Header("Animation Refs")]
    [SerializeField] private NetworkAnimator _naRef;
    //[SerializeField] private AnimationClip _attack;

    [Header("Camera Params")]
    public float PivotHeight;
    public float BoomLength;


    [Header("Movement Specs")]
    public float Gravity;
    public float MouseSense = 5f;
    public float Speed = 5f;
    public float JumpForce;
    public float TurnSmoothVelocity;
    private float VerticalLook = 0f;

    [SerializeField] private float _mouseXFactor = 4f;
    [SerializeField] private float _mouseYFactor = 4f;

    [Header("Dash Traits")]
    public float DashDistance;
    public float DashCooldown;
    public float DashDuration;
    private float _lastDash;
    private float _dashScale = 2f;
    
    [Header("Dodge Traits")]
    public float DodgeDistance;
    public float DodgeCooldown;
    public float DodgeDuration;
    private float _lastDodge;
    private float _dodgeScale = 1.5f;
    private float _verticalVelocity = 0f;

    private float _movementTimestep = .005f;

    [Header("Attack Traits")] 
    public float MaxTimeBetweenAttacks = 1.5f;
    public float[] AttackSegmentCooldown = {0, .2f,.2f,1.1f};
    public float[] AttackSegmentDuration = { .1f, .1f, 1f };
    private int _currentAttack = 0;
    private float _lastAttack = 0f;
    
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
        if (IsOwner)
        {
            // Owner enables character controller when spawned
            if (_ccRef != null)
            {
                _ccRef.enabled = true;
            }
            _cmRef.gameObject.SetActive(true);
            _cbRef.transform.localPosition = new Vector3(0f, PivotHeight, 0f);
            _cfRef.transform.localPosition = new Vector3(0f, 0f, -BoomLength);
            _piRef.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            _cmRef.gameObject.SetActive(false);
        }

        base.OnNetworkSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentAttack > 0 && Time.time - _lastAttack > 1.5f)
        {
            Debug.Log("reset attack state");
            _currentAttack = 0;
            _naRef.Animator.SetInteger("AttackState",_currentAttack);
        }
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
        _naRef.Animator.SetBool("isGrounded",_pState.Contains(PlayerState.Grounded));
        var delta = Mouse.current.delta.ReadValue();

        var move = _pState.Contains(PlayerState.ControlLocked) ? _piRef.actions["Move"].ReadValue<Vector2>() * .1f : _piRef.actions["Move"].ReadValue<Vector2>();
        _naRef.Animator.SetBool("SpeedNotZero", move != Vector2.zero);
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

    public void CheckForUpdatedSense()
    {
        MouseSense = PlayerPrefs.GetFloat("MouseSensitivity",1);
        Debug.Log(MouseSense);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox") && other.gameObject.GetComponent<AgentCharacter>().m_Agent.m_TeamID !=
            GetComponent<AgentCharacter>().m_Agent.m_TeamID)
        {
            GetComponent<AgentCharacter>().ModifyHealth(-1);
        }
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

           
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, transform.eulerAngles.y + (delta.x * MouseSense * _mouseXFactor * Time.deltaTime), ref TurnSmoothVelocity, 0f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

        }

    }

    private void HandleVertLook(Vector2 delta)
    {
        if (delta.y == 0)
        {
            return;
        }
        VerticalLook -= delta.y * MouseSense * _mouseYFactor * Time.deltaTime;
        VerticalLook = Mathf.Clamp(VerticalLook, -85f, 85f);
        _cbRef.transform.localRotation = Quaternion.Euler(new Vector3(VerticalLook, 0f, 0f));
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
            _verticalVelocity -= Gravity * Time.deltaTime;
            //add a max 
        }
        else
        {
            _verticalVelocity = -_ccRef.stepOffset;
        }
        dir.y = _verticalVelocity;
        _ccRef.Move(Time.deltaTime * dir);
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
        _naRef.Animator.SetTrigger("Jumped");
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
        _naRef.Animator.SetBool("DashingOrDodging",true);
        _naRef.Animator.SetTrigger("DashedOrDodged");
        _pState.Add(PlayerState.Dashed);
        float dashedDist = 0;
        while (dashedDist < DashDistance)
        {
            //Debug.Log("movepertick " + movePerTick + " and in dir " + dir);
            float moveThisTick = Time.deltaTime * Speed * _dashScale;
            dashedDist += moveThisTick;
            _ccRef.Move( moveThisTick * dir);
            yield return new WaitForSeconds(0);
        }

        _pState.Remove(PlayerState.Dashed);
        _naRef.Animator.SetTrigger("DashOrDodgeEnd");
        _naRef.Animator.SetBool("DashingOrDodging",false);
        _pState.Add(PlayerState.ControlLocked);
        Invoke(nameof(ClearControlLock),.5f);
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
        _naRef.Animator.SetBool("DashingOrDodging",true);
        _naRef.Animator.SetTrigger("DashedOrDodged");
        _pState.Add(PlayerState.Dodged);
        float dodgedDist = 0;
        
        while (dodgedDist < DodgeDistance)
        {
            float moveThisFrame = Time.deltaTime * Speed ;
            dodgedDist += moveThisFrame;
            _ccRef.Move(moveThisFrame * dir);
            yield return new WaitForSeconds(0);
        }

        _pState.Remove(PlayerState.Dodged);
        _naRef.Animator.SetTrigger("DashOrDodgeEnd");
        _naRef.Animator.SetBool("DashingOrDodging",false);
        
        yield break;
    }

    [ServerRpc]
    private void DodgeServerRpc(CardinalDirections dir)
    {
        Dodge(dir);
    }

    public void OnFire()
    {
        
        if (/*IsServer && */IsLocalPlayer && Time.time - _lastAttack > AttackSegmentCooldown[_currentAttack])
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
        _pState.Add(PlayerState.ControlLocked);
        _lastAttack = Time.time;
        //_naRef.Animator.Play(_attack);
        _currentAttack++;
        Debug.Log(_currentAttack);
        _naRef.Animator.SetInteger("AttackState", _currentAttack);
        Invoke(nameof(ClearControlLock), AttackSegmentDuration[_currentAttack-1]);
        //_atkAnim.Play();
    }

    private void ClearControlLock()
    {
        _pState.Remove(PlayerState.ControlLocked);
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
