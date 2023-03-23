using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

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
        if (CTF.WinService.m_IsGameOver) return;
        Assert.IsTrue(toAdd > 0);
        m_Score += toAdd;
        CTF.TeamService.UpdateScoreClientRpc(m_TeamID, m_Score);
        CTF.WinService.IsGameOver();
    }

    public override string ToString() => $"Team {m_TeamID}";
}

public class TeamService : NetworkBehaviour
{
    private List<Team> m_Teams;
    
    [ServerRpc]
    public void AddScoreServerRpc(int teamID, float addScore)
    {
        this.GetTeam(teamID).AddScore(addScore);
    }
    
    [ServerRpc]
    public void GetScoreServerRpc(int teamID)
    {
        float score = 0;
        if (DoesTeamExist(teamID))
        {
            score = m_Teams[teamID].m_Score;
        }
        CTF.TeamService.UpdateScoreClientRpc(teamID, score);
    }

    [ClientRpc]
    public void UpdateScoreClientRpc(int teamID, float score)
    {
        if (GameCondUI.Instance != null)
        {
            GameCondUI.Instance.UpdateScore(teamID, score);
        }
    }


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


    public void OnAwake()
    {
        m_Teams = new List<Team>();
        for (int i = 0; i < CTF.Instance.TeamCount; i++)
        {
            CreateTeam();
        }

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
