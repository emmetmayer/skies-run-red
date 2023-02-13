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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
