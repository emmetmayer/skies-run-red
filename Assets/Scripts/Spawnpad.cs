using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpad : MonoBehaviour
{
    [SerializeField] public int TeamID;

    private BoxCollider boxCollider;


    public Vector3 GetSpawnPosition()
    {
        float rX = Random.Range(-1.0f, 1.0f) * (boxCollider.size.x / 2.0f);
        float rY = Random.Range(-1.0f, 1.0f) * (boxCollider.size.y / 2.0f);
        float rZ = Random.Range(-1.0f, 1.0f) * (boxCollider.size.z / 2.0f);
        return this.transform.position + new Vector3(rX, rY, rZ);
    }


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
