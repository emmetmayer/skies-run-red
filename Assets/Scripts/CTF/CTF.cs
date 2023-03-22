using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CTF : NetworkBehaviour
{
    public static CTF Instance {get; private set;}

    public static TeamService TeamService {get; private set;}
    public static AgentService AgentService {get; private set;}
    public static WinService WinService {get; private set;}
    public static GameTimer GameTimer {get; private set;}

    public bool IsRunning = false;

    [SerializeField] private float TotalGameTime = 300;
    [SerializeField] public int MaxScore = 10;
    [SerializeField] public int TeamCount = 2;

    public void StartGame()
    {
        if (IsRunning) return;
        IsRunning = true;

        CTF.GameTimer.SetTimeLeft(TotalGameTime);
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

            TeamService = new TeamService();
            TeamService.OnAwake();

            AgentService = new AgentService();
            AgentService.OnAwake();

            WinService = new WinService();
            WinService.OnAwake();

            GameTimer = this.GetComponent<GameTimer>();
            GameTimer.OnAwake();

            return true;
        }
    }

    void Awake()
    {
        if (!DoSingleton()) return;
    }
}
