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

    public NetworkVariable<bool> IsRunning = new NetworkVariable<bool>(false);

    [SerializeField] private float TotalGameTime = 300;
    [SerializeField] public int MaxScore = 10;
    [SerializeField] public int TeamCount = 2;

    private bool isStarting = false;
    public void StartGame()
    {
        if (!IsServer) return;
        if (IsRunning.Value) return;
        if (isStarting) return;
        isStarting = true;
        
        TeamService.OnAwake();
        AgentService.OnAwake();
        WinService.OnAwake();
        GameTimer.OnAwake();

        CTF.GameTimer.SetTimeLeft(TotalGameTime);
        
        IsRunning.Value = true;
    }

    [ServerRpc]
    public void StartGameServerRPC()
    {
        StartGame();
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

            TeamService = this.GetComponent<TeamService>();
            AgentService = this.GetComponent<AgentService>();
            WinService = this.GetComponent<WinService>();
            GameTimer = this.GetComponent<GameTimer>();

            return true;
        }
    }

    void Awake()
    {
        DoSingleton();
    }
}
