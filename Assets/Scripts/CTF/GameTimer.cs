using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameTimer : NetworkBehaviour
{
    private float m_TimeLeftFloat = 0;
    public NetworkVariable<int> m_TimeLeft = new NetworkVariable<int>(0);

    public float GetTimeLeft()
    {
        return m_TimeLeft.Value;
    }

    public void SetTimeLeft(float seconds)
    {
        m_TimeLeftFloat = seconds;
        m_TimeLeft.Value = (int)Mathf.Ceil(seconds);
    }
    


    public void OnAwake()
    {
        
    }
    
    void Update()
    {
        if (!IsServer) return;
        if (!CTF.Instance.IsRunning.Value) return;

        m_TimeLeftFloat = Mathf.Max(m_TimeLeftFloat - Time.deltaTime, 0);

        int newTimeLeftInt = (int)Mathf.Ceil(m_TimeLeftFloat);
        if (newTimeLeftInt != m_TimeLeft.Value) m_TimeLeft.Value = newTimeLeftInt;

        if (m_TimeLeftFloat <= 0)
        {
            CTF.WinService.IsGameOver();
        }
    }
}
