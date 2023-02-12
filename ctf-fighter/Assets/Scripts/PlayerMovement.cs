using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;

public enum PlayerState
{
    NONE,
    ControlLocked, //use this when you dont want the player to move
    Grounded,
    Airborne,
    
    //i can do like. sliding/double-jumped but im not sure
    //might need to do this for multi step abilities/basic combos
}
public class PlayerMovement : MonoBehaviour
{
    private PlayerState _state;

    [SerializeField] private CharacterController _ccRef;
    [SerializeField] private Rigidbody _rbRef;
    [SerializeField] private PlayerInput _piRef;

    public float Speed = 5f;
    public float MouseXSense = 5f;

    private float _mouseXFacter = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    // Update is called once per frame
    void Update()
    {
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
            if (Pointer.current.enabled)
            {
                Debug.Log(Pointer.current.delta);
                Debug.Log(Pointer.current.delta.ReadValue());
               
                Debug.Log(Pointer.current.position);
                Debug.Log(Pointer.current.position.ReadValue());
                
                Debug.Log(Pointer.current.pressure);
                Debug.Log(Pointer.current.pressure.ReadValue());
                
            }
            var delta = Pointer.current.delta.ReadValue();
            
            if (delta.sqrMagnitude == 0)
            {
                return;
            }
            //Debug.Log(delta);
            //delta *= 0.5f; // Account for scaling applied directly in Windows code by old input system.
            //delta *= 0.1f; // Account for sensitivity setting on old Mouse X and Y axes.
            //transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.y + Time.deltaTime * delta.x, 0f));
            int dir = delta.x > 0 ? 1 : -1;

            //Debug.Log("neg");
            //transform.rotation = Quaternion.AngleAxis(dir * MouseXSense * _mouseXFacter, Vector3.up);
          
            transform.Rotate(new Vector3(0f, dir * MouseXSense * _mouseXFacter,0f), Space.Self);
            

        }
        
        //reset mouse pos - unnecessary 
        
        //use movement to rotate player
        

        //sVector2 v = value.Get<Vector2>();
        //value.
        

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
        
        _rbRef.velocity =  Speed * dir;
    }
    
    public void OnJump()
    {
        //set y velocity
    }
    
    public void OnSlide()
    {
        
    }
    public void OnCrouch()
    {
        
    }
    //figure out how to take directional
    public void OnDash()
    {
        //lock input? do fast movement
    }
    public void OnDodge(InputValue value)
    {
        //lock input? do short, fast movement
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
