using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AgentCharacter : NetworkBehaviour
{
    // Abstract agent reference
    [SerializeField] public Agent m_Agent {get; private set;}

    // Character metadata
    [SerializeField] private float m_MaxHealth = 100.0f;
    [SerializeField] private float m_Health = 100.0f;
    [SerializeField] public Flag m_HeldFlag;
    private bool m_IsDead = false;

    public float MaxHealth { get { return m_MaxHealth; } private set { m_MaxHealth = value; } }
    public float Health { get { return m_Health; } private set { m_Health = value; } }

    //theres probably multiple of both of these?
    [Tooltip("When this collides with the enemy's hitbox they take damage")]
    private List<Collider> _hurtBox;
    [Tooltip("When this collides with the enemy's hurtbox I take damage")]
    private List<Collider> _hitBox;
    private bool isDefending = false;
    
    public void Update()
    {
        if (m_Health <= 0)
        {
            OnDied(); // TODO: REMOVE LATER
        }
    }

    public void OnDied()
    {
        if (m_IsDead) return;
        m_IsDead = true;

        if (m_HeldFlag)
        {
            m_HeldFlag.Drop();
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

        if (healthChange > 0 && m_Health < 0)
        {
            OnDied();
        }
        
        return healthChange;
    }


    // Constructor
    public void New(Agent agent)
    {
        m_Agent = agent;
        m_Health = m_MaxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        
        if (other.tag == "FlagStand")
        {
            FlagStand flagStand = other.GetComponent<FlagStand>();
            if (!flagStand) return;

            if (flagStand.m_TeamID == m_Agent.m_TeamID)
            {
                if (m_HeldFlag)
                {
                    m_HeldFlag.ScorePoints(m_Agent.m_TeamID);
                }
            }
            else
            {
                Transform flagObject = other.transform.Find("Flag");
                if (flagObject)
                {
                    ulong networkObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;
                    flagObject.GetComponent<Flag>().GrabServerRpc(networkObjectId);
                }
            }
        }

        if (other.tag == "Flag")
        {
            ulong networkObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;
            other.GetComponent<Flag>().GrabServerRpc(networkObjectId);
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
