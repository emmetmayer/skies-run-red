using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    [SerializeField] public string Name {get; private set;}
    [SerializeField] public int TeamID {get; private set;}
    [SerializeField] public GameObject Character {get; private set;}

    public Agent(string _name, int _teamID)
    {
        Name = _name;
        TeamID = _teamID;
        Spawn();
    }

    public void LoadCharacter()
    {
        if (Character)
        {
            GameObject.Destroy(Character);
        }
        Character = GameObject.Instantiate(Resources.Load("Character", typeof(GameObject)), GameObject.Find("Agents").transform) as GameObject;
    }

    private void Spawn()
    {
        TeamService.Instance.SpawnAgent(this);
    }

    public override string ToString()
    {
        return Name;
    }
}
