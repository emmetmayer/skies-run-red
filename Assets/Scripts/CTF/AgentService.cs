using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AgentService : MonoBehaviour
{
    public static AgentService Instance {get; private set;}

    [SerializeField] private List<Agent> m_Agents;
    public Transform m_AgentContainer {get; private set;}

    public Agent AddAgent(ulong _clientId, string _name, int _teamID = -1)
    {
        string name = _name;
        if (_teamID == -1) _teamID = (m_Agents.Count % 2);
        int teamID = _teamID;

        Agent newAgent = new Agent(_clientId, name, teamID);
        m_Agents.Add(newAgent);
        return newAgent;
    }

    private bool DoSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return false;
        }
        else
        {
            Instance = this;
            return true;
        }
    }

    void Awake()
    {
        if (!DoSingleton()) return;

        m_Agents = new List<Agent>();
        m_AgentContainer = (new GameObject("AgentContainer")).transform;
    }
}
