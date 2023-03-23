using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CTFUI : NetworkBehaviour
{
    [SerializeField] GameObject m_CTFUI;
    
    public override void OnNetworkSpawn()
    {
        m_CTFUI.SetActive(CTF.Instance.IsRunning.Value);
        CTF.Instance.IsRunning.OnValueChanged += (bool previous, bool current) => {
            m_CTFUI.SetActive(CTF.Instance.IsRunning.Value);
        };
    }
}
