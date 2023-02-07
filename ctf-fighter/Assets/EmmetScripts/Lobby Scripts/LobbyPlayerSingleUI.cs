using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyPlayerSingleUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI playerNameText;

    private Player player;


    private void Awake() {
        
    }


    public void UpdatePlayer(Player player) {
        this.player = player;
        playerNameText.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
    }

}