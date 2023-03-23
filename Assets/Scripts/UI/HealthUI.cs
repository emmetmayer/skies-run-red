using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthUI : MonoBehaviour
{
    [SerializeField] RectTransform m_HealthBar;
    private float m_MaxWidth;
    private float m_LastHealth;

    // TEMP CHAR TRACKING
    NetworkObject playerObject;
    AgentCharacter agentCharacter;
    

    void DoUIUpdate()
    {
        if (agentCharacter && agentCharacter.Health != m_LastHealth)
        {
            m_LastHealth = agentCharacter.Health;
            float newWidth = Mathf.Clamp((agentCharacter.Health / agentCharacter.MaxHealth) * m_MaxWidth, 0f, m_MaxWidth);
            m_HealthBar.sizeDelta = new Vector2(newWidth, m_HealthBar.rect.height);
        }
    }
    

    void UpdatePlayerRef()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.LocalClient == null) return;
        NetworkObject currentPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (currentPlayerObject != playerObject)
        {
            playerObject = currentPlayerObject;
            if (playerObject != null)
            {
                agentCharacter = playerObject.GetComponent<AgentCharacter>();
                m_LastHealth = agentCharacter.MaxHealth;
            }
        }
    }

    void Start()
    {
        UpdatePlayerRef();

        m_MaxWidth = m_HealthBar.rect.width;

        DoUIUpdate();
    }

    void LateUpdate()
    {
        UpdatePlayerRef();
        DoUIUpdate();
    }
}
