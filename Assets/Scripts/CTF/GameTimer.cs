using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance {get; private set;}
    
    float time_left = 0;

    float GetTimeLeft()
    {

    }

    void SetTimeLeft(int seconds)
    {
        time_left = seconds;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time_left = Mathf.Max(time_left - Time.deltaTime, 0);
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
