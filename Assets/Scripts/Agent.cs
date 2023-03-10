using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    [SerializeField] public string Name {get; private set;}
    [SerializeField] public int TeamID {get; private set;}
    [SerializeField] public GameObject Character {get; private set;}

    private float Health;
    //theres probably multiple of both of these?
    [Tooltip("When this collides with the enemy's hitbox they take damage")]
    private Collider _hurtBox;
    [Tooltip("When this collides with the enemy's hurtbox I take damage")]
    private Collider _hitBox;

    private bool isDefending;
    
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
        //grab all the relevant colliders here
    }

    private void Spawn()
    {
        TeamService.Instance.SpawnAgent(this);
        //should this run LoadCharacter here?
    }

    public override string ToString()
    {
        return Name;
    }

    //return true if damage caused player to die
    public bool TakeDamage(int n)
    {
        if (isDefending)
        {
            //process attack differently
        }

        if(n > 0) Health -= n;
        if (Health < 0)
        {
            HandleDeath();
            return true;
        }

        return false;
    }

    public void HandleDeath()
    {
        //spawn timer
        //body ragdoll?
        //reset values
    }
    
    //gotta figure out how to do collision events?
    
}
