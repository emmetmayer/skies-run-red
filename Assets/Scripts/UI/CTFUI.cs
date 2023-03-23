using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFUI : MonoBehaviour
{
    [SerializeField] GameObject m_CTFUI;
    
    void Start()
    {
        m_CTFUI.SetActive(CTF.Instance.IsRunning.Value);
        CTF.Instance.IsRunning.OnValueChanged += (bool previous, bool current) => {
            m_CTFUI.SetActive(CTF.Instance.IsRunning.Value);
        };
    }
}
