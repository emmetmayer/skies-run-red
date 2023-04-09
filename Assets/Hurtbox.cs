using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Hurtbox : NetworkBehaviour
{
    [SerializeField] [Range(0f,100f)] private float damageOnTrigger = 10f;
    [SerializeField] [Range(0f,100f)] private float damageOnCollide = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsClient) return;
        if (other.tag == "Player")
        {
            AgentCharacter agentChar = other.GetComponent<AgentCharacter>();
            agentChar.ModifyHealth(-damageOnTrigger);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsClient) return;
        Collider other = collision.collider;
        if (other.tag == "Player")
        {
            AgentCharacter agentChar = other.GetComponent<AgentCharacter>();
            agentChar.ModifyHealth(-damageOnCollide);
        }
    }
}
