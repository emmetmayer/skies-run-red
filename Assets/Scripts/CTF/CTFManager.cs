using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFManager : MonoBehaviour
{
    public static CTFManager Instance {get; private set;}

    public float GetTimeLeft()
    {
        return GameTimer.Instance.GetTimeLeft();
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
