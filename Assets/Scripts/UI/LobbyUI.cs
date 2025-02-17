using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private TMP_Text lobbyNameText;
    [SerializeField]
    private GameObject lobbyPlayerPrefab;
    [SerializeField]
    private Transform lobbyPlayerParent;

    public void OpenLobby(Lobby lobby)
    {
        lobbyNameText.text = lobby.Name;

        foreach (Player lobbyPlayer in lobby.Players)
        {
            LobbyPlayerUI lobbyPlayerUI =
                Instantiate(lobbyPlayerPrefab, new Vector3(), Quaternion.identity, lobbyPlayerParent)
                    .GetComponent<LobbyPlayerUI>();
            lobbyPlayerUI.SetPlayer(lobbyPlayer);
        }

        gameObject.SetActive(true);
    }
}