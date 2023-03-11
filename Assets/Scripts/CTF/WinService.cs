using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinService : MonoBehaviour
{
    public static WinService Instance {get; private set;}

    bool HasTeamWonGame()
    {
        if (GameTimer.Instance.GetTimeLeft() <= 0)
        {
            return true;
        }

        return false;
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
}
