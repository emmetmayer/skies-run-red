using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpad : MonoBehaviour
{
    [SerializeField] public int m_TeamID;
    private BoxCollider m_BoxCollider;


    public Vector3 GetSpawnPosition()
    {
        float playerHeight = 1.0f;
        float rX = Random.Range(-1.0f, 1.0f) * (m_BoxCollider.size.x / 2.0f);
        float rY = Random.Range(-1.0f, 1.0f) * (m_BoxCollider.size.y / 2.0f);
        float rZ = Random.Range(-1.0f, 1.0f) * (m_BoxCollider.size.z / 2.0f);
        return this.transform.position + new Vector3(rX, rY + playerHeight, rZ);
    }


    void Awake()
    {
        m_BoxCollider = GetComponent<BoxCollider>();
    }
}
