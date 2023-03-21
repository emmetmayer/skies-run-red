using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameTimer : NetworkBehaviour
{
    private float m_TimeLeft = 0;

    public float GetTimeLeft()
    {
        return m_TimeLeft;
    }

    public void SetTimeLeft(float seconds)
    {
        m_TimeLeft = seconds;
    }
    


    public void OnAwake()
    {
        
    }
    
    void Update()
    {
        if (!CTF.Instance.IsRunning) return;

        m_TimeLeft = Mathf.Max(m_TimeLeft - Time.deltaTime, 0);
        if (m_TimeLeft <= 0)
        {
            CTF.WinService.IsGameOver();
        }
    }
}
