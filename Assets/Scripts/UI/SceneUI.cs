using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


public enum SceneState
{
    None,
    Menu,
    Creation,
    Lobby,
    Game
}

public class SceneUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TMP_InputField playerNameInputField;
    [SerializeField]
    private Button playerSignInButton;

    [Header("Panels")]
    [SerializeField]
    private GameObject startMenuUI;
    [SerializeField]
    private MainMenuUI mainMenuUI;
    [SerializeField]
    private LobbyUI lobbyUI;
    [SerializeField]
    private CreateLobbyUI createLobbyUI;

    public SceneState SceneState { get; private set; } = SceneState.None;

    public async void OnAuthJoinButtonClick()
    {
        playerSignInButton.interactable = false;

        string playerName = playerNameInputField.text;
        if (string.IsNullOrEmpty(playerName)) return;

        bool signedIn = await AuthManager.Instance.InitializeAuth(playerName);

        if (signedIn)
        {
            SceneState = SceneState.Menu;
            startMenuUI.SetActive(false);
            mainMenuUI.gameObject.SetActive(true);
        }

        playerSignInButton.interactable = true;
    }

    public void OpenLobby(Lobby lobby)
    {
        SceneState = SceneState.Lobby;
        mainMenuUI.gameObject.SetActive(false);
        lobbyUI.OpenLobby(lobby);
    }
}