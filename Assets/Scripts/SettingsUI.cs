using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
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

    private float _lastSens = 0;

    private CursorLockMode _prevState;
    private void Start()
    {
        //set slider values to what the stored setting is
        _panel.SetActive(false);
        _lastSens = PlayerPrefs.GetFloat("MouseSensitivity",1);
        _mouseSenseSlider.SetValueWithoutNotify(_lastSens);
        _mouseSenseText.text = ""+_mouseSenseSlider.value;
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
        Debug.Log(_lastSens);
        //update local mouse sense
        if (Mathf.Abs(_mouseSenseSlider.value - _lastSens) > float.Epsilon)
        {
            PlayerPrefs.SetFloat("MouseSensitivity", _mouseSenseSlider.value);
            _lastSens = _mouseSenseSlider.value;
            try
            {
                GameObject inScene = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject;
                if (inScene)
                {
                    Debug.Log("check for updated sense");
                    inScene.GetComponent<PlayerMovement>().CheckForUpdatedSense();
                }
            }
            catch (NullReferenceException e)
            {
                Debug.Log("could not find something necessary for update check");
            }
            //GameObject inScene = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject;
            
        }
        
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
