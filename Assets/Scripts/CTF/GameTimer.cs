using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance {get; private set;}
    
    [SerializeField] private float time_left = 0;

    public float GetTimeLeft()
    {
        return time_left;
    }

    void SetTimeLeft(int seconds)
    {
        time_left = seconds;
    }
    

    private bool DoSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return false;
        }
        else
        {
            Instance = this;
            return true;
        }
    }

    void Awake()
    {
        if (!DoSingleton()) return;
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
}
