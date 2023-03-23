using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Flag : NetworkBehaviour
{
    // Holder metadata
    [SerializeField] private AgentCharacter m_CurrentHolder;
    [SerializeField] private Transform m_HoldingObject;
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

        //this.transform.SetParent(m_Stand);
        m_HoldingObject = m_Stand;
        this.transform.position = m_StandOffset;
        this.transform.rotation = m_OriginalRot;

        m_IsOnStand = true;
        m_StoredPoints = 1;
    }

    public void ScorePoints(int byTeamID)
    {
        CTF.TeamService.AddScoreServerRpc(byTeamID, m_StoredPoints);
        ReturnToStand();
    }

    public void Grab(AgentCharacter agentChar)
    {
        if (!IsServer) return;
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
            m_HoldingObject = agentChar.transform;
            //this.transform.SetParent(agentChar.transform);

            m_IsOnStand = false;
        }
        else if (!m_IsOnStand)
        {
            ReturnToStand();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GrabServerRpc(ulong networkObjectId)
    {
        NetworkObject agentCharObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        AgentCharacter agentChar = agentCharObject.GetComponent<AgentCharacter>();
        Grab(agentChar);
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

        //this.transform.parent = null;
        //m_HoldingObject = null;
        this.transform.position -= m_FlagOffset;
    }
    

    void Awake()
    {
        if (!IsServer) return;

        m_Stand = this.transform.parent;

        ReturnToStand();
    }

    void Update()
    {
        if (!IsServer) return;

        if (m_CurrentHolder)
        {
            m_CurrentHoldTime += Time.deltaTime;
        }

        if (m_HoldingObject != null)
        {
            this.transform.position = m_HoldingObject.position;
        }
    }
}
