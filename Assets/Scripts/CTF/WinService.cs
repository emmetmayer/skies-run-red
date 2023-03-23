using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WinService : NetworkBehaviour
{
    public bool m_IsGameOver {get; private set;}

    public int GetMaxScore()
    {
        return CTF.Instance.MaxScore;
    }

    void GetWinningTeam()
    {
        float highestScore = 0;
        int winningTeamID = -1;

        List<Team> allTeams = CTF.TeamService.GetAllTeams();
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

        List<Team> allTeams = CTF.TeamService.GetAllTeams();
        for (int i = 0; i < allTeams.Count; i++)
        {
            if (allTeams[i].m_Score >= CTF.Instance.MaxScore)
            {
                m_IsGameOver = true;
                GetWinningTeam();
                return true;
            }
        }

        if (CTF.GameTimer.GetTimeLeft() <= 0)
        {
            m_IsGameOver = true;
            GetWinningTeam();
            return true;
        }

        return false;
    }
    

    public void OnAwake()
    {
        
    }
}
