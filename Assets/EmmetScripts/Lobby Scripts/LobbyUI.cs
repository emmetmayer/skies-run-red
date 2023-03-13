using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {


    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate1;
    [SerializeField] private Transform playerSingleTemplate2;
    [SerializeField] private Transform container1;
    [SerializeField] private Transform container2;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveLobbyButton;


    private void Awake() {
        Instance = this;

        playerSingleTemplate1.gameObject.SetActive(false);
        playerSingleTemplate2.gameObject.SetActive(false);

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
        });

        startGameButton.onClick.AddListener(() => {
            LobbyManager.Instance.StartGame();
        });
        


    }

    private void Start() {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnGameStarted += LobbyManager_OnGameStarted;

        Hide();
    }

    private void LobbyManager_OnGameStarted(object sender, System.EventArgs e)
    {
        ClearLobby();
        Hide();
        transform.parent.gameObject.SetActive(false);
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();

        foreach (Player player in lobby.Players) {
            Transform playerSingleTransform = null;
            if (player.Data[LobbyManager.KEY_PLAYER_TEAM].Value == "0")
            {
                playerSingleTransform = Instantiate(playerSingleTemplate1, container1);
            }
            else if (player.Data[LobbyManager.KEY_PLAYER_TEAM].Value == "1")
            {
                playerSingleTransform = Instantiate(playerSingleTemplate2, container2);
            }

            playerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.UpdatePlayer(player);
        }

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

        Show();
    }

    private void ClearLobby() {
        foreach (Transform child in container1) 
        {
            if (child == playerSingleTemplate1) continue;
            Destroy(child.gameObject);
        }
        foreach (Transform child in container2)
        {
            if (child == playerSingleTemplate2) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}