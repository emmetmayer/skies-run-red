using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance {get; private set;}
    
    [Range(0, 600)]
    [SerializeField] private float m_TimeLeft = 0;

    public float GetTimeLeft()
    {
        return m_TimeLeft;
    }

    void SetTimeLeft(int seconds)
    {
        m_TimeLeft = seconds;
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
    
    void Update()
    {
        m_TimeLeft = Mathf.Max(m_TimeLeft - Time.deltaTime, 0);
    }
}
