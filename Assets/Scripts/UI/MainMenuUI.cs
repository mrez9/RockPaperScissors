using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private SceneUI sceneUI;
    [SerializeField]
    private List<Button> bottomButtons;
    [SerializeField]
    private GameObject lobbyItemPrefab;
    [SerializeField]
    private Transform lobbyItemsParent;

    private List<Button> allButtons = new();

    // interval to update list of given lobbies while player is in lobby-select panel
    private float lobbiesUpdateTimeInveravl = 2f;

    private float nextLobbiesUpdateTime;

    public void OpenMainMenu(List<Lobby> lobbies)
    {
        allButtons = new();
        allButtons.AddRange(bottomButtons);

        FillLobbies(lobbies);
    }

    private async void Update()
    {
        try
        {
            if (sceneUI.SceneState == SceneState.Menu)
            {
                if (Time.realtimeSinceStartup >= nextLobbiesUpdateTime)
                {
                    //prevent trigger new update during updating lobbies
                    nextLobbiesUpdateTime = float.MaxValue;

                    Debug.Log($"trying to update lobby lists");
                    var lobbies = await LobbyManager.Instance.RetrieveLobbyLists();

                    // if player is not in lobby-selection menu, ignore the result
                    if (this == null || sceneUI.SceneState != SceneState.Menu)
                    {
                        return;
                    }

                    FillLobbies(lobbies);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void FillLobbies(List<Lobby> lobbies)
    {
        // remove existing lobbies in ui
        foreach (Transform child in lobbyItemsParent)
        {
            Destroy(child.gameObject);
        }

        allButtons = new();
        allButtons.AddRange(bottomButtons);

        if (lobbies is { Count: > 0 })
        {
            foreach (Lobby lobby in lobbies)
            {
                LobbyUIItem lobbyUIItem =
                    Instantiate(lobbyItemPrefab, new Vector3(), Quaternion.identity, lobbyItemsParent)
                        .GetComponent<LobbyUIItem>();
                Button joinLobbyButton = lobbyUIItem.SetLobby(lobby, sceneUI, this);
                allButtons.Add(joinLobbyButton);
            }
        }

        nextLobbiesUpdateTime = Time.realtimeSinceStartup + lobbiesUpdateTimeInveravl;
    }

    public async void OnHostButtonClick()
    {
        try
        {
            SetButtonsInteractable(false);

            var relayJoinCode = await MultiplayerManager.Instance.InitializeHost();
            if (string.IsNullOrEmpty(relayJoinCode))
            {
                SetButtonsInteractable(true);
                return;
            }

            var lobby = await LobbyManager.Instance.CreateLobby(relayJoinCode);
            if (lobby == null)
            {
                SetButtonsInteractable(true);
                return;
            }

            sceneUI.OpenLobby(lobby);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    #region Intractable

    public void SetButtonsInteractable(bool isInteractable)
    {
        foreach (Button button in allButtons)
        {
            button.interactable = isInteractable;
        }
    }

    #endregion
}