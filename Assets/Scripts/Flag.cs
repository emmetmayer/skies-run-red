using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    // Holder metadata
    [SerializeField] private AgentCharacter m_CurrentHolder;
    [SerializeField] private Vector3 m_FlagOffset;

    private Transform m_Stand;
    private Vector3 m_StandOffset;
    private Quaternion m_OriginalRot;
    private bool m_IsOnStand;

    // CTF data
    [SerializeField] public int m_TeamID;
    [SerializeField] private float m_StoredPoints = 1f;
    [SerializeField] private float m_CurrentHoldTime;


    void ReturnToStand()
    {
        if (m_CurrentHolder)
        {
            m_CurrentHolder.m_HeldFlag = null;
            m_CurrentHolder = null;
        }

        this.transform.SetParent(m_Stand);
        this.transform.position = m_StandOffset;
        this.transform.rotation = m_OriginalRot;

        m_IsOnStand = true;
        m_StoredPoints = 1;
    }

    public void ScorePoints(int byTeamID)
    {
        TeamService.Instance.GetTeam(byTeamID).AddScore(m_StoredPoints);
        ReturnToStand();
    }

    public void Grab(AgentCharacter agentChar)
    {
        if (m_CurrentHolder) return;
        if (agentChar.m_HeldFlag) return;

        if (agentChar.m_Agent.m_TeamID != m_TeamID)
        {
            m_CurrentHolder = agentChar;
            agentChar.m_HeldFlag = this;
            m_CurrentHoldTime = 0;
            
            this.transform.position = agentChar.transform.position + m_FlagOffset;
            Vector3 ea = agentChar.transform.rotation.eulerAngles;
            this.transform.rotation = Quaternion.Euler(ea.x, ea.y + 90, ea.z + 90);
            this.transform.SetParent(agentChar.transform);

            m_IsOnStand = false;
        }
        else if (!m_IsOnStand)
        {
            ReturnToStand();
        }
        
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
    

    void Awake()
    {
        m_Stand = this.transform.parent;
        m_StandOffset = this.transform.position;
        m_OriginalRot = this.transform.rotation;
        m_IsOnStand = true;
    }

    void Update()
    {
        if (m_CurrentHolder)
        {
            m_CurrentHoldTime += Time.deltaTime;
        }
    }
}
