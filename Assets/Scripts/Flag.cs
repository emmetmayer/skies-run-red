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
    private ulong m_NetworkObjectId;

    public NetworkVariable<bool> m_IsOnStand = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> m_IsBeingHeld = new NetworkVariable<bool>(false);

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
        
        m_IsBeingHeld.Value = false;
        m_Flag.position = m_StandPosition;
        m_Flag.rotation = m_StandRotation;

        m_IsOnStand.Value = true;
        m_StoredPoints = 1;
    }

    [ServerRpc(RequireOwnership=false)]
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

        if (agentChar.m_Agent.m_TeamID.Value != m_TeamID)
        {
            m_CurrentHolder = agentChar;
            m_CurrentHoldTime = 0;
            
            agentChar.m_HeldFlag = this;
            agentChar.SetHeldFlagClientRpc(m_NetworkObjectId, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { agentChar.m_Agent.m_ClientID.Value }
                }
            });

            m_IsBeingHeld.Value = true;
            m_IsOnStand.Value = false;
        }
        else if (!m_IsOnStand.Value)
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

        m_CurrentHolder.m_HeldFlag = null;
        m_CurrentHolder.SetHeldFlagClientRpc(0, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { m_CurrentHolder.m_Agent.m_ClientID.Value }
            }
        });
        
        // If there's ground below the drop point, drop at this position
        // If there's nothing under the drop point, respawn at the flag stand
        Vector3 origin = m_CurrentHolder.transform.position;
        m_CurrentHolder = null;
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 10.0f, 0))
        {
            m_IsBeingHeld.Value = false;
            m_Flag.position = hit.point + new Vector3(0, 1f, 0);
        }
        else
        {
            ReturnToStand();
        }
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
        if (IsServer)
        {
            m_StandPosition = m_Flag.position;
            m_StandRotation = m_Flag.rotation;
            m_NetworkObjectId = this.GetComponent<NetworkObject>().NetworkObjectId;

            ReturnToStand();
        }
        m_IsBeingHeld.OnValueChanged += (bool previous, bool current) => {
            m_Flag.gameObject.SetActive(!current);
        };
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
