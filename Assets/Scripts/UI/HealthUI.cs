using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] RectTransform m_HealthBar;
    private float m_MaxWidth;
    private float m_LastHealth;

    [Range(0, 100)]
    [SerializeField] float debug_currentHealth = 50.0f;
    [SerializeField] float debug_maxHealth = 100.0f;

    void DoUIUpdate()
    {
        //float currentHealth = 50.0f; // TODO: Get local player health
        if (debug_currentHealth != m_LastHealth)
        {
            //float maxHealth = 100.0f; // TODO: Get local player health
            m_LastHealth = debug_currentHealth;
            float newWidth = Mathf.Clamp((debug_currentHealth / debug_maxHealth) * m_MaxWidth, 0f, m_MaxWidth);
            m_HealthBar.sizeDelta = new Vector2(newWidth, m_HealthBar.rect.height);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_MaxWidth = m_HealthBar.rect.width;
        m_LastHealth = debug_maxHealth; // TODO: Get local player health
        DoUIUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        DoUIUpdate();
    }
}
