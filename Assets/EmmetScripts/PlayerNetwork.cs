using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float speed = 5.0f;

    private CharacterController controller;

    public override void OnNetworkSpawn()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (IsServer && IsLocalPlayer)
        {
            Move(input);
        } 
        else if (IsLocalPlayer)
        {
            MoveServerRpc(input);
        }

    }

    private void Move(Vector2 movementInput)
    {
        Vector3 movement = movementInput.x * transform.right + movementInput.y * transform.forward;
        if(movement != Vector3.zero)
        {
            controller.Move(movement * Time.deltaTime * speed);
        }
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 movementInput)
    {
        Move(movementInput);
    }
}
