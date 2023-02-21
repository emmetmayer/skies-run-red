using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [SerializeField] private int TeamID;

    public int GetTeamID() {return TeamID;}
    public void SetTeamID(int teamID) {TeamID = teamID;}

    // Start is called before the first frame update
    void Start()
    {
        TeamService.Instance.SpawnPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
