using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

// Agent is an abstraction for each player connected to the game session
// It contains information about the player (e.g. clientID, Name, Team)
// However, it is server-authoritative (the client doesn't have control over it)
public class Agent : NetworkBehaviour
{
    public AgentCharacter m_AgentCharacter;

    public NetworkVariable<ulong> m_ClientID = new NetworkVariable<ulong>(0);
    public NetworkVariable<FixedString64Bytes> m_ClientName = new NetworkVariable<FixedString64Bytes>("DEFAULT");
    public NetworkVariable<int> m_TeamID = new NetworkVariable<int>(0);
    public float m_RespawnTime = 5.0f;
    
    public void OnCreate(ulong clientId, string name, int teamID)
    {
        m_ClientID.Value = clientId;
        m_ClientName.Value = name;
        m_TeamID.Value = teamID;
        OnNetworkSpawn();
    }

    public void LoadCharacter()
    {
        if (!IsServer) return;
        
        if (m_AgentCharacter)
        {
            m_AgentCharacter.GetComponent<NetworkObject>().Despawn(true);
            m_AgentCharacter = null;
        }

        var spawn_info = CTF.TeamService.GetSpawnPosition(this);

        GameObject m_Character = GameObject.Instantiate(Resources.Load("Character", typeof(GameObject))) as GameObject;
        m_Character.transform.position = spawn_info.position;
        m_Character.transform.rotation = spawn_info.rotation;
        m_Character.GetComponent<NetworkObject>().SpawnAsPlayerObject(m_ClientID.Value);
        m_Character.transform.parent = this.transform;

        m_AgentCharacter = m_Character.GetComponent<AgentCharacter>();
        m_AgentCharacter.m_IsServerReady.Value = true;
    }

    public IEnumerator Died()
    {
        yield return new WaitForSeconds(m_RespawnTime);
        LoadCharacter();
    }

    [ServerRpc(RequireOwnership=false)]
    public void DiedServerRpc()
    {
        StartCoroutine(Died());
    }
    
    public string GetName()
    {
        return m_ClientName.Value.ToString();
    }

    public override string ToString()
    {
        return GetName();
    }

    public override void OnNetworkSpawn()
    {
        this.gameObject.name = "AGENT_" + m_ClientName.Value;
    }
}
