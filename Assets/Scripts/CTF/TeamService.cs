using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public Team(int teamID)
    {
        TeamID = teamID;
        Spawnpads = new List<Spawnpad>();
    }

    public int TeamID {get; private set;}
    public List<Spawnpad> Spawnpads {get; private set;}

    public override string ToString() => $"Team {TeamID}";
}

public class TeamService : MonoBehaviour
{
    public static TeamService Instance {get; private set;}
    private List<Team> teams;

    bool DoesTeamExist(int TeamID)
    {
        return TeamID <= teams.Count;
    }

    int CreateTeam()
    {
        int newTeamID = teams.Count + 1; // 0 == no team
        Team newTeam = new Team(newTeamID);
        teams.Add(newTeam);
        return newTeamID;
    }

    Team GetTeam(int TeamID)
    {
        return teams[TeamID-1];
    }

    public bool JoinTeam(PlayerMain player, int TeamID)
    {
        // Useless right now, all players are automatically set to teams
        /*
        if (!DoesTeamExist(TeamID))
        {
            return false;
        }
        if (!IsPlayerOnTeam(player))
        {
            return false;
        }
        player.SetTeamID(TeamID);
        */
        return true;
    }

    bool IsPlayerOnTeam(PlayerMain A)
    {
        return A.GetTeamID() != 0;
    }

    bool ArePlayersOnSameTeam(PlayerMain A, PlayerMain B)
    {
        return IsPlayerOnTeam(A) && A.GetTeamID() == B.GetTeamID();
    }


    public void SpawnPlayer(PlayerMain player)
    {
        int TeamID = player.GetTeamID();
        if (!DoesTeamExist(TeamID))
        {
            return;
        }

        Team team = GetTeam(TeamID);
        int numSpawnpads = team.Spawnpads.Count;
        Spawnpad randomSpawnpad = team.Spawnpads[Random.Range(0, numSpawnpads-1)];
        Vector3 spawn_position = randomSpawnpad.GetSpawnPosition();
        player.transform.position = spawn_position;
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

        teams = new List<Team>();
        CreateTeam(); // TeamID: 1
        CreateTeam(); // TeamID: 2

        var allSpawnpads = Object.FindObjectsOfType<Spawnpad>();
        for (int i = 0; i < allSpawnpads.Length; i++)
        {
            int TeamID = allSpawnpads[i].TeamID;
            if (!DoesTeamExist(TeamID))
            {
                continue;
            }
            GetTeam(TeamID).Spawnpads.Add(allSpawnpads[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
