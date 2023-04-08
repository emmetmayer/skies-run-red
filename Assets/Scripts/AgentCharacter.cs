using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AgentCharacter : NetworkBehaviour
{
    // Abstract agent reference
    [SerializeField] public Agent m_Agent {get; private set;}
    private ulong m_NetworkObjectId;

    // Character metadata
    [SerializeField] private float m_MaxHealth = 100.0f;
    [SerializeField] private float m_Health = 100.0f;
    [SerializeField] public Flag m_HeldFlag;
    [SerializeField] public GameObject m_FlagObject;
    private bool m_IsDead = false;

    public float MaxHealth { get { return m_MaxHealth; } private set { m_MaxHealth = value; } }
    public float Health { get { return m_Health; } private set { m_Health = value; } }

    //theres probably multiple of both of these?
    [Tooltip("When this collides with the enemy's hitbox they take damage")]
    private List<Collider> _hurtBox;
    [Tooltip("When this collides with the enemy's hurtbox I take damage")]
    private List<Collider> _hitBox;
    private bool isDefending = false;
    
    public void OnDied()
    {
        if (m_IsDead) return;
        m_IsDead = true;

        GetComponent<PlayerMovement>().enabled = false;

        if (m_HeldFlag)
        {
            m_HeldFlag.DropServerRpc(m_NetworkObjectId);
        }
        
        // TODO: ragdoll or other death effect
        StartCoroutine(m_Agent.Died());
    }

    //QUESTION: should we just localize all of take damage here? 
    //since this stores both the colliders for damage and the health it probably makes more sense
    public float ModifyHealth(float amount)
    {
        if (isDefending)
        {
            //process attack differently
        }

        float newHealth = m_Health + amount;
        m_Health = Mathf.Clamp(newHealth, 0, m_MaxHealth);
        float healthChange = Mathf.Abs(newHealth - m_Health);

        if (healthChange > 0 && m_Health <= 0)
        {
            OnDied();
        }
        
        return healthChange;
    }

    [ClientRpc]
    public void SetHeldFlagClientRpc(ulong flagNetworkId, ClientRpcParams clientRpcParams = default)
    {
        Flag newHeldFlag = null;
        if (flagNetworkId != 0)
        {
            NetworkObject flagObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[flagNetworkId];
            newHeldFlag = flagObject.GetComponent<Flag>();
        }

        bool isHeld = (newHeldFlag != null);
        m_FlagObject.SetActive(isHeld);
        m_HeldFlag = newHeldFlag;
    }


    // Constructor
    public void New(Agent agent)
    {
        m_Agent = agent;
        m_Health = m_MaxHealth;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        m_NetworkObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        
        if (other.tag == "FlagStand")
        {
            Flag flag = other.GetComponent<Flag>();
            if (flag.m_TeamID == m_Agent.m_TeamID)
            {
                if (m_HeldFlag)
                {
                    m_HeldFlag.ScorePointsServerRpc(m_Agent.m_TeamID);
                }
            }
            else
            {
                flag.GrabServerRpc(m_NetworkObjectId);
            }
        }

        if (other.tag == "Flag" && !m_HeldFlag)
        {
            Flag flag = other.transform.parent.GetComponent<Flag>();
            flag.GrabServerRpc(m_NetworkObjectId);
        }

        if (other.CompareTag("Hitbox"))
        {
            ModifyHealth(-10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        
        bool hit = collision.collider.CompareTag("Hitbox");
        
        //throw new NotImplementedException();)
    }
}
