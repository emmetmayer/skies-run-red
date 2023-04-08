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
        string name = _name;
        if (_teamID == -1) _teamID = (m_Agents.Count % 2);
        int teamID = _teamID;

        Agent newAgent = Instantiate(AgentPrefab).GetComponent<Agent>();
        newAgent.GetComponent<NetworkObject>().Spawn();

        newAgent.gameObject.name = "AGENT_"+name;
        newAgent.OnCreate(_clientId, name, teamID);
        newAgent.transform.parent = CTF.Instance.transform;

        m_Agents.Add(newAgent);

        return newAgent;
    }

    public void OnAwake()
    {
        m_Agents = new List<Agent>();
    }
}
