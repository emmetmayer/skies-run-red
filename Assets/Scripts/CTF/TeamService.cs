using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamService : MonoBehaviour
{
    public static TeamService Instance {get; private set;}

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

    // Start is called before the first frame update
    void Start()
    {
        
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
