using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    [SerializeField] public AgentCharacter m_Character {get; private set;}
    [SerializeField] public string m_Name {get; private set;}
    [SerializeField] public int m_TeamID {get; private set;}

    [SerializeField] public float m_RespawnTime {get; private set;} = 5.0f;
    
    public Agent(string name, int teamID)
    {
        m_Name = name;
        m_TeamID = teamID;

        LoadCharacter();
        TeamService.Instance.SpawnAgent(this);
    }

    public void LoadCharacter()
    {
        if (m_Character)
        {
            GameObject.Destroy(m_Character);
        }
        GameObject newCharacter = GameObject.Instantiate(Resources.Load("Character", typeof(GameObject)), AgentService.Instance.m_AgentContainer) as GameObject;
        m_Character = newCharacter.GetComponent<AgentCharacter>();
        m_Character.New(this);
    }


    IEnumerator Died()
    {
        m_Character.OnDied();
        yield return new WaitForSeconds(m_RespawnTime);
        LoadCharacter();
    }

    // Returns true if damage caused player to die
    public bool TakeDamage(float n)
    {
        if (!m_Character) return false;

        float healthChange = m_Character.ModifyHealth(-n);
        if (healthChange > 0 && m_Character.m_Health < 0)
        {
            CTFManager.Instance.StartCoroutine(Died());
            return true;
        }

        return false;
    }
    

    public override string ToString()
    {
        return m_Name;
    }
}
