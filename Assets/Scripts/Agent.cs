using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Agent
{
    [SerializeField] public GameObject m_Character {get; private set;}
    [SerializeField] public AgentCharacter m_AgentCharacter {get; private set;}
    [SerializeField] public ulong m_ClientID {get; private set;}
    [SerializeField] public string m_Name {get; private set;}
    [SerializeField] public int m_TeamID {get; private set;}

    [SerializeField] public float m_RespawnTime {get; private set;} = 5.0f;
    
    public Agent(ulong clientId, string name, int teamID)
    {
        m_ClientID = clientId;
        m_Name = name;
        m_TeamID = teamID;
    }
    

    public void LoadCharacter()
    {
        if (m_Character)
        {
            GameObject.Destroy(m_Character);
        }

        m_Character = GameObject.Instantiate(Resources.Load("CharacterRoot", typeof(GameObject)), CTF.AgentService.m_AgentContainer) as GameObject;
        m_AgentCharacter = m_Character.transform.Find("Character").GetComponent<AgentCharacter>();
        m_AgentCharacter.New(this);
        m_Character.GetComponent<NetworkObject>().SpawnAsPlayerObject(m_ClientID);
    }

    public IEnumerator Died()
    {
        yield return new WaitForSeconds(m_RespawnTime);
        LoadCharacter();
    }
    

    public override string ToString()
    {
        return m_Name;
    }
}
