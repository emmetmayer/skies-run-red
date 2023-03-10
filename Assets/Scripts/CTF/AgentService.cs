using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentService : MonoBehaviour
{
    public static AgentService Instance {get; private set;}
    [SerializeField] private List<Agent> agents;

    void AddAgent(string _name, int _teamID = -1)
    {
        string name = _name;
        if (_teamID == -1) _teamID = (agents.Count % 2);
        int teamID = _teamID;

        Agent newPlayer = new Agent(name, teamID);
        agents.Add(newPlayer);
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

        agents = new List<Agent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AddAgent("PLAYER_1");
        AddAgent("PLAYER_2");
        AddAgent("PLAYER_3");
        AddAgent("PLAYER_4");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
