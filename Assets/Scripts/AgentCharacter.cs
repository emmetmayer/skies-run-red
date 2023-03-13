using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCharacter : MonoBehaviour
{
    // Abstract agent reference
    [SerializeField] public Agent m_Agent {get; private set;}

    // Character metadata
    [SerializeField] private float m_MaxHealth = 100.0f;
    [SerializeField] public float m_Health {get; private set;}

    [SerializeField] public Flag m_HeldFlag;

    //theres probably multiple of both of these?
    [Tooltip("When this collides with the enemy's hitbox they take damage")]
    private List<Collider> _hurtBox;
    [Tooltip("When this collides with the enemy's hurtbox I take damage")]
    private List<Collider> _hitBox;
    private bool isDefending = false;

    public void Start()
    {

    }

    public void OnDied()
    {
        if (m_HeldFlag)
        {
            m_HeldFlag.Drop();
        }
        // TODO: ragdoll or other death effect
        CTFManager.Instance.StartCoroutine(m_Agent.Died());
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
                    flagObject.GetComponent<Flag>().Grab(this);
                }
            }
        }

        if (other.tag == "Flag")
        {
            other.GetComponent<Flag>().Grab(this);
        }

        if (other.CompareTag("Hitbox"))
        {
            ModifyHealth(-10);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool hit = collision.collider.CompareTag("Hitbox");
        
        //throw new NotImplementedException();)
    }
}
