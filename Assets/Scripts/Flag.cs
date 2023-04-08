using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Flag : NetworkBehaviour
{
    // Holder metadata
    [SerializeField] private AgentCharacter m_CurrentHolder;
    [SerializeField] private Transform m_Flag;
    private Vector3 m_StandPosition;
    private Quaternion m_StandRotation;
    private bool m_IsOnStand;
    private ulong m_NetworkObjectId;

    // CTF data
    [SerializeField] public int m_TeamID;
    [SerializeField] private float m_StoredPoints = 1f;
    [SerializeField] private float m_CurrentHoldTime;


    void ReturnToStand()
    {
        if (m_CurrentHolder)
        {
            Drop();
        }
        
        m_Flag.gameObject.SetActive(true);
        m_Flag.position = m_StandPosition;
        m_Flag.rotation = m_StandRotation;

        m_IsOnStand = true;
        m_StoredPoints = 1;
    }

    [ServerRpc]
    public void ScorePointsServerRpc(int byTeamID)
    {
        CTF.TeamService.AddScore(byTeamID, m_StoredPoints);
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
            m_CurrentHoldTime = 0;
            
            agentChar.m_HeldFlag = this;
            agentChar.SetHeldFlagClientRpc(m_NetworkObjectId, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { agentChar.m_Agent.m_ClientID }
                }
            });

            m_Flag.gameObject.SetActive(false);
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

    public void Drop()
    {
        if (!m_CurrentHolder) return;

        m_Flag.gameObject.SetActive(true);
        m_Flag.position = m_CurrentHolder.transform.position;

        m_CurrentHolder.m_HeldFlag = null;
        m_CurrentHolder.SetHeldFlagClientRpc(0, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { m_CurrentHolder.m_Agent.m_ClientID }
            }
        });
        
        m_CurrentHolder = null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DropServerRpc(ulong networkObjectId)
    {
        NetworkObject agentCharObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        AgentCharacter agentChar = agentCharObject.GetComponent<AgentCharacter>();
        if (agentChar != m_CurrentHolder) return;
        Drop();
    }
    

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        m_StandPosition = m_Flag.position;
        m_StandRotation = m_Flag.rotation;
        m_NetworkObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;

        ReturnToStand();
    }

    void Update()
    {
        if (!IsServer) return;

        if (m_CurrentHolder)
        {
            m_CurrentHoldTime += Time.deltaTime;
        }
    }
}
