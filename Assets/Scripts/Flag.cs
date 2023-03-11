using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    // Holder metadata
    [SerializeField] private AgentCharacter m_CurrentHolder;
    [SerializeField] private Vector3 m_FlagOffset;

    // CTF data
    [SerializeField] public int m_TeamID;
    [SerializeField] private float m_StoredPoints;
    [SerializeField] private float m_CurrentHoldTime;


    public void Grab(AgentCharacter agentChar)
    {
        if (m_CurrentHolder) return;
        if (agentChar.m_Agent.m_TeamID == m_TeamID) return;
        if (agentChar.m_HeldFlag) return;
        m_CurrentHolder = agentChar;
        agentChar.m_HeldFlag = this;

        m_CurrentHoldTime = 0;
        this.transform.SetParent(agentChar.transform);
        this.transform.position = agentChar.transform.position + m_FlagOffset;
    }

    public void Throw()
    {
        if (!m_CurrentHolder) return;
        m_CurrentHolder.m_HeldFlag = null;
        m_CurrentHolder = null;

        // TODO
    }

    public void Drop()
    {
        if (!m_CurrentHolder) return;
        m_CurrentHolder.m_HeldFlag = null;
        m_CurrentHolder = null;

        this.transform.parent = null;
        this.transform.position -= m_FlagOffset;
    }
    

    void Update()
    {
        if (m_CurrentHolder)
        {
            m_CurrentHoldTime += Time.deltaTime;
        }
    }
}
