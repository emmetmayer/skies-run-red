using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinService : MonoBehaviour
{
    public static WinService Instance {get; private set;}

    public bool m_IsGameOver {get; private set;}

    public int GetMaxScore()
    {
        return CTF.Instance.MaxScore;
    }

    void GetWinningTeam()
    {
        float highestScore = 0;
        int winningTeamID = -1;

        List<Team> allTeams = TeamService.Instance.GetAllTeams();
        for (int i = 0; i < allTeams.Count; i++)
        {
            if (allTeams[i].m_Score >= highestScore)
            {
                highestScore = allTeams[i].m_Score;
                winningTeamID = i;
            }
        }

        if (winningTeamID > -1)
        {
            Debug.Log("TEAM " + winningTeamID + " WINS!");
        }
        else
        {
            Debug.Log("Nobody won!");
        }
    }

    public bool IsGameOver()
    {
        if (m_IsGameOver) return true;

        List<Team> allTeams = TeamService.Instance.GetAllTeams();
        for (int i = 0; i < allTeams.Count; i++)
        {
            if (allTeams[i].m_Score >= CTF.Instance.MaxScore)
            {
                m_IsGameOver = true;
                GetWinningTeam();
                return true;
            }
        }

        if (GameTimer.Instance.GetTimeLeft() <= 0)
        {
            m_IsGameOver = true;
            GetWinningTeam();
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
