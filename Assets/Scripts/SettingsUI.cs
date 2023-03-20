using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private PlayerMovement _pmRef;

    [SerializeField] private GameObject _panel;
    
    [SerializeField] private TMP_Text _mouseSenseText;
    [SerializeField] private Slider _mouseSenseSlider;
    [SerializeField] private TMP_Text _volumeText;
    [SerializeField] private Slider _volumeSlider;


    private CursorLockMode _prevState;
    private void Start()
    {
        //set slider values to what the stored setting is
        _panel.SetActive(false);
    }
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            var isActive = _panel.activeSelf;
            _panel.SetActive(!isActive);
            //unlock mouse
            if (isActive) Cursor.lockState = _prevState;
            else
            {
                _prevState = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;
            }
            //activate panel
        }
    }
    
    public void UpdateMouseSense()
    {
        
        _mouseSenseText.text = ""+_mouseSenseSlider.value;
        //update local mouse sense
    }
    
    public void UpdateVolume()
    {
        _volumeText.text = ""+_volumeSlider.value;
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    
}
