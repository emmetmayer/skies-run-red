using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Agent : NetworkBehaviour
{
    [SerializeField] public GameObject m_Character {get; private set;}
    [SerializeField] public AgentCharacter m_AgentCharacter {get; private set;}
    [SerializeField] public ulong m_ClientID {get; private set;}
    [SerializeField] public string m_Name {get; private set;}
    [SerializeField] public int m_TeamID {get; private set;}

    [SerializeField] public float m_RespawnTime {get; private set;} = 5.0f;
    
    public void OnCreate(ulong clientId, string name, int teamID)
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

        Vector3 spawn_position = CTF.TeamService.GetSpawnPosition(this);

        m_Character = GameObject.Instantiate(Resources.Load("Character", typeof(GameObject))) as GameObject;
        m_AgentCharacter = m_Character.GetComponent<AgentCharacter>();
        m_AgentCharacter.New(this);
        m_Character.transform.position = spawn_position;
        m_Character.GetComponent<NetworkObject>().SpawnAsPlayerObject(m_ClientID);
        m_Character.transform.parent = this.transform;
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
