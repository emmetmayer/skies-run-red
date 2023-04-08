using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Button LobbyListButton;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _textBox;

    public void Awake()
    {
        LobbyListButton.onClick.AddListener(() => {
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
        });
    }

    public void SetName()
    {
        //bring up the set name input UI
        
    }

    public void OpenLobbyList()
    {
        //bring up lobby ui
    }

    public void Credits()
    {
        _textBox.text = "";
    }

    public void Quit()
    {
        Application.Quit();
    }
}
