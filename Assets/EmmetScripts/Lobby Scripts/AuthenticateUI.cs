using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour {


    [SerializeField] private Button authenticateButton;
    private GameObject lobbyList;

    private void Awake() {
        authenticateButton.onClick.AddListener(() => {
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            //Hide();
        });
        lobbyList = GameObject.Find("LobbyListUI");
        lobbyList.SetActive(false);
    }

    public void enableNecessaryUI()
    {
        lobbyList.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }

}