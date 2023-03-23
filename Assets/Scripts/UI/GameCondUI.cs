using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameCondUI : MonoBehaviour
{
    public static GameCondUI Instance;

    [SerializeField] TextMeshProUGUI m_Timer;
    [SerializeField] TextMeshProUGUI m_ScoreTeamA;
    [SerializeField] TextMeshProUGUI m_ScoreTeamB;

    string ConvertSToMS(float time)
    {
        float seconds = Mathf.Floor(time % 60f);
        float minutes = Mathf.Floor(time / 60f);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }
    

    public void UpdateScore(int teamID, float newScore)
    {
        int maxScore = CTF.Instance.MaxScore;
        switch (teamID)
        {
            case 0:
                m_ScoreTeamA.text = string.Format("Team A: {0} / {1}", newScore, maxScore);
                break;
            case 1:
                m_ScoreTeamB.text = string.Format("Team A: {0} / {1}", newScore, maxScore);
                break;
            default:
                break;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
        m_Timer.text = ConvertSToMS(CTF.GameTimer.m_TimeLeft.Value);
        CTF.GameTimer.m_TimeLeft.OnValueChanged += (int previous, int current) => {
            m_Timer.text = ConvertSToMS(current);
        };
        
        for (int i = 0; i < CTF.Instance.TeamCount; i++)
        {
            CTF.TeamService.GetScoreServerRpc(i);
        }
    }
}
