using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;

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
    
    //i can do like. sliding/double-jumped but im not sure
    //might need to do this for multi step abilities/basic combos
}
public enum CardinalDirections
{
    Forward,
    Backward,
    Left,
    Right
}
public class PlayerMovement : MonoBehaviour
{
    private ControlState _cState;
    private PlayerState _pState;

    [Header("References to relevant components")]
    [SerializeField] private CharacterController _ccRef;
    [SerializeField] private Rigidbody _rbRef;
    [SerializeField] private PlayerInput _piRef;

    [Header("Movement Specs")]
    public float Gravity;
    public float MouseXSense = 5f;
    public float Speed = 5f;
    public float Jump;
    
    private float _mouseXFactor = 1f;
    
    [Header("Dash Traits")]
    public float DashDistance;
    public float DashCooldown;
    private float _lastDash;
    
    [Header("Dodge Traits")]
    public float DodgeDistance;
    public float DodgeCooldown;
    private float _lastDodge;
    private float _verticalVelocity = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    // Update is called once per frame
    void Update()
    { 
        _pState = _ccRef.isGrounded ? PlayerState.Grounded : PlayerState.Airborne;
        
        HandleLook();
        //default player move
        HandleMove();
        
        //any external mods
        
        //send it to server
        
        
        //_dir = transform.forward;
        //transform.position = new Vector3(0, 0, 0);
    }
    
    
#region Input

    private void HandleLook()
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
            var delta = Mouse.current.delta.ReadValue();
            //Debug.Log(delta);
            if (delta.x == 0)
            {
                return;
            }
            
            //delta *= 0.5f; // Account for scaling applied directly in Windows code by old input system.
            //delta *= 0.1f; // Account for sensitivity setting on old Mouse X and Y axes.

            int dir = delta.x > 0 ? 1 : -1;

            transform.Rotate(new Vector3(0f, dir * MouseXSense * _mouseXFactor,0f), Space.Self);
            

        }

    }


    //it would be fine to just . call this in update actually i will do that
    private void HandleMove()
    {
        //this currently only knows about keyboard
        var move = _piRef.actions["Move"].ReadValue<Vector2>();
        var dir = Vector3.zero;
        if (move.x > 0)
        {
            dir += transform.right;
        }
        if (move.x < 0 )
        {
            dir -= transform.right;
        }
        if (move.y > 0)
        {
            dir += transform.forward;
        }
        if (move.y < 0 )
        {
            dir -= transform.forward;
        }

        dir = new Vector3(dir.x, 0, dir.z);
        dir = dir.normalized;
        dir *= Speed;
        //this should be couched into a grounded check probably
        if (_verticalVelocity > 0 || !_ccRef.isGrounded)
        {
            _verticalVelocity -= Gravity;
            //add a max 
        }
        else
        {
            _verticalVelocity = 0;
        }
        //Debug.Log(_verticalVelocity);
        dir.y = _verticalVelocity;
        _ccRef.Move(Time.deltaTime* dir);
        //_rbRef.velocity =  Speed * dir;
    }
    
    public void OnJump()
    {
        //set y velocity
        if (_cState == ControlState.DoubleJumped || _cState == ControlState.ControlLocked)
        {
            return;
        }
        _verticalVelocity = Jump;
        if (_pState == PlayerState.Airborne) _cState = ControlState.DoubleJumped;
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
        Dash(CardinalDirections.Forward);
    }
    public void OnBackwardDash()
    {
        //lock input? do fast movement
        Dash(CardinalDirections.Backward);
    }
    public void OnLeftDash()
    {
        //lock input? do fast movement
        Dash(CardinalDirections.Left);
    }
    public void OnRightDash()
    {
        //lock input? do fast movement
        Dash(CardinalDirections.Right);
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
        _ccRef.Move(Time.deltaTime* moveDir * DashDistance);
    }
    public void OnForwardDodge()
    {
        Dodge(CardinalDirections.Forward);
    }
    public void OnBackwardDodge()
    {
        Dodge(CardinalDirections.Backward);
    }
    public void OnLeftDodge()
    {
        Dodge(CardinalDirections.Left);
    }
    public void OnRightDodge()
    {
        Dodge(CardinalDirections.Right);
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
        _ccRef.Move(Time.deltaTime* moveDir * Speed * DodgeDistance); 
    }
    public void OnFire()
    {
        
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
