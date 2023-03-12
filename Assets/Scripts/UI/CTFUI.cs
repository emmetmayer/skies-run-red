using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CTFUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Timer;
    [SerializeField] TextMeshProUGUI m_ScoreTeamA;
    [SerializeField] TextMeshProUGUI m_ScoreTeamB;

    string ConvertSToMS(float time)
    {
        float seconds = Mathf.Floor(time % 60f);
        float minutes = Mathf.Floor(time / 60f);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }

    void DoUIUpdate()
    {
        m_Timer.text = ConvertSToMS(GameTimer.Instance.GetTimeLeft());
        m_ScoreTeamA.text = string.Format("Team A: {0}", TeamService.Instance.GetTeam(0).m_Score);
        m_ScoreTeamB.text = string.Format("Team B: {0}", TeamService.Instance.GetTeam(1).m_Score);
    }

    // Start is called before the first frame update
    void Start()
    {
        DoUIUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        DoUIUpdate();
    }
}
