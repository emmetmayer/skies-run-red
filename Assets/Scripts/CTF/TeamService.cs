using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamService : MonoBehaviour
{
    public static TeamService Instance {get; private set;}

    private Dictionary<int, List<Spawnpad>> spawnpads;


    int CreateTeam()
    {
        // TODO
        return 0; // Placeholder
    }

    bool ArePlayersOnSameTeam(/*Player A, Player B*/)
    {
        // TODO
        return true;
    }

    public void SpawnPlayer(GameObject player)
    {
        int playerTeamID = 0; // TODO: Replace with actual team id once valid

        if (spawnpads.ContainsKey(playerTeamID))
        {
            int numSpawnpads = spawnpads[playerTeamID].Count;
            Spawnpad randomSpawnpad = spawnpads[playerTeamID][Random.Range(0, numSpawnpads-1)];
            Vector3 spawn_position = randomSpawnpad.GetSpawnPosition();
            player.transform.position = spawn_position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnpads = new Dictionary<int, List<Spawnpad>>();

        var allSpawnpads = Object.FindObjectsOfType<Spawnpad>();
        for (int i = 0; i < allSpawnpads.Length; i++)
        {
            int ID = allSpawnpads[i].TeamID;
            if (!spawnpads.ContainsKey(ID))
            {
                spawnpads.Add(ID, new List<Spawnpad>());
            }
            spawnpads[ID].Add(allSpawnpads[i]);
        }

        var allPlayers = Object.FindObjectsOfType<PlayerNetwork>();
        foreach (var player in allPlayers)
        {
            SpawnPlayer(player.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Singleton Setup
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
