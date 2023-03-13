using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Team
{
    public Team(int teamID)
    {
        m_TeamID = teamID;
        m_Spawnpads = new List<Spawnpad>();
        m_Score = 0;
    }

    public int m_TeamID {get; private set;}
    public List<Spawnpad> m_Spawnpads {get; private set;}
    public float m_Score {get; private set;}

    public void AddScore(float toAdd)
    {
        if (WinService.Instance.m_IsGameOver) return;
        Assert.IsTrue(toAdd > 0);
        m_Score += toAdd;
        WinService.Instance.IsGameOver();
    }

    public override string ToString() => $"Team {m_TeamID}";
}

public class TeamService : MonoBehaviour
{
    public static TeamService Instance {get; private set;}
    
    private List<Team> m_Teams;

    bool DoesTeamExist(int TeamID)
    {
        return TeamID >= 0 && TeamID < m_Teams.Count;
    }

    int CreateTeam()
    {
        int newTeamID = m_Teams.Count; // -1 == no team
        Team newTeam = new Team(newTeamID);
        m_Teams.Add(newTeam);
        return newTeamID;
    }

    public Team GetTeam(int TeamID)
    {
        return m_Teams[TeamID];
    }

    public List<Team> GetAllTeams()
    {
        return m_Teams;
    }

    bool IsPlayerOnTeam(Agent A)
    {
        return A.m_TeamID != -1;
    }

    bool ArePlayersOnSameTeam(Agent A, Agent B)
    {
        return IsPlayerOnTeam(A) && A.m_TeamID == B.m_TeamID;
    }


    public void SpawnAgent(Agent agent)
    {
        int TeamID = agent.m_TeamID;
        if (!DoesTeamExist(TeamID))
        {
            return;
        }

        if (!agent.m_Character)
        {
            agent.LoadCharacter();
        }

        Team team = GetTeam(TeamID);
        int numSpawnpads = team.m_Spawnpads.Count;
        Assert.IsTrue(numSpawnpads > 0);
        Spawnpad randomSpawnpad = team.m_Spawnpads[Random.Range(0, numSpawnpads-1)];
        Vector3 spawn_position = randomSpawnpad.GetSpawnPosition();
        agent.m_Character.transform.position = spawn_position;
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

        m_Teams = new List<Team>();
        CreateTeam(); // TeamID: 0
        CreateTeam(); // TeamID: 1

        var allSpawnpads = Object.FindObjectsOfType<Spawnpad>();
        for (int i = 0; i < allSpawnpads.Length; i++)
        {
            int TeamID = allSpawnpads[i].m_TeamID;
            if (!DoesTeamExist(TeamID))
            {
                continue;
            }
            GetTeam(TeamID).m_Spawnpads.Add(allSpawnpads[i]);
        }
    }
}
