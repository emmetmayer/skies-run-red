using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AgentService : NetworkBehaviour
{
    [SerializeField] private GameObject AgentPrefab;
    [SerializeField] private List<Agent> m_Agents;

    public Agent AddAgent(ulong _clientId, string _name, int _teamID = -1)
    {
        Agent newAgent = Instantiate(AgentPrefab).GetComponent<Agent>();
        newAgent.GetComponent<NetworkObject>().Spawn();
        newAgent.OnCreate(_clientId, _name, _teamID);
        
        newAgent.transform.parent = CTF.Instance.transform;

        m_Agents.Add(newAgent);

        return newAgent;
    }

    public void OnAwake()
    {
        m_Agents = new List<Agent>();
    }
}
