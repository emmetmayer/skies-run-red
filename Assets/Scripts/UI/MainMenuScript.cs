using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Button LobbyListButton;
    [SerializeField] private Button ChangeNameButton;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _textBox;

    //ServerList and ChangeName currently use localised scripts on their respective buttons, written from some netcode tutorials
    public void Credits()
    {
        _textBox.text = "";
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void DisableWhenGameStarts()
    {
        gameObject.SetActive(false);
    }
}
