using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIItem : MonoBehaviour
{
    [Header("Scene UI References")]
    private SceneUI sceneUI;
    private MainMenuUI mainMenuUI;

    [Header("UI References")]
    [SerializeField]
    private TMP_Text lobbyNameText;
    [SerializeField]
    private TMP_Text lobbyHostNameText;
    [SerializeField]
    private TMP_Text playersCountText;
    [SerializeField]
    private Button joinButton;

    private string lobbyId;

    public Button SetLobby(Lobby lobby, SceneUI sceneUI, MainMenuUI mainMenuUI)
    {
        this.sceneUI = sceneUI;
        this.mainMenuUI = mainMenuUI;

        lobbyNameText.text = lobby.Name;
        lobbyHostNameText.text = lobby.Data[LobbyManager.keyLobbyHost].Value;
        playersCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

        lobbyId = lobby.Id;

        // return join button to add in interactable list
        return joinButton;
    }

    public async void OnJoinLobbyButtonClick()
    {
        mainMenuUI.SetButtonsInteractable(false);

        var lobbyJoined = await LobbyManager.Instance.JoinLobby(lobbyId);

        if (this == null || lobbyJoined == null) return;

        var lobbyRelayJoinCode = lobbyJoined.Data[LobbyManager.keyLobbyRelayJoinCode].Value;
        await MultiplayerManager.Instance.InitializeClient(lobbyRelayJoinCode);

        sceneUI.OpenLobby(lobbyJoined);
    }
}