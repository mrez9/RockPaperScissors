using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TMP_Text playerNameText;
    [SerializeField]
    private Image playerReadyImage;
    [SerializeField]
    private Color32[] playerReadyColors; // 0:ready, 1:not ready


    public void SetPlayer(Player player)
    {
        playerNameText.text = player.Data[LobbyManager.keyPlayerName].Value;
        bool isReady = bool.Parse(player.Data[LobbyManager.keyPlayerReady].Value);
        playerReadyImage.color = isReady ? playerReadyColors[0] : playerReadyColors[1];
    }
}