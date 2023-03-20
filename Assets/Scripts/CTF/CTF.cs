using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTF : MonoBehaviour
{
    public static CTF Instance {get; private set;}

    public bool IsRunning = false;

    [SerializeField] private float TotalGameTime = 300;
    [SerializeField] public int MaxScore = 10;
    [SerializeField] public int TeamCount = 2;

    public void StartGame()
    {
        if (IsRunning) return;
        IsRunning = true;

        GameTimer.Instance.SetTimeLeft(TotalGameTime);
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
