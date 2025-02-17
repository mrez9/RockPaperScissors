using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Static Data")]
    [SerializeField]
    [Tooltip("max size of showing lobbies")]
    private int maxLobbiesToList = 20;
    [SerializeField]
    [Tooltip("Interval for heartbeat the lobby to avoid going as inactive")]
    private int hostHeartbeatFrequency = 20;

    // Lobby Data
    // Key for lobby's host name
    public const string keyLobbyHost = "lobby_host_name";
    // Key for lobby's relay join code
    public const string keyLobbyRelayJoinCode = "lobby_relay_join_code";

    // Player Data
    // key for player name
    public const string keyPlayerName = "player_name";
    // Key to retrieve player ready from player lobby data
    public const string keyPlayerReady = "player_ready";


    // public List<Lobby> lobbies { get; private set; } = new();

    public Lobby activeLobby { get; private set; }

    public List<Player> inLobbyPlayers { get; private set; }

    public bool isHost { get; private set; }

    private bool isGameStarted = false;
    private bool isPlayerReady = false;

    float nextHeartbeatTime;
    float nextUpdatePlayersTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private async void Update()
    {
        try
        {
            if (activeLobby != null && !isGameStarted)
            {
                if (isHost && Time.realtimeSinceStartup >= nextHeartbeatTime)
                {
                    await PeriodicHostHeartbeat();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    #region Periodic Updates

    async Task PeriodicHostHeartbeat()
    {
        try
        {
            // Set next heartbeat time before calling Lobby Service
            nextHeartbeatTime = Time.realtimeSinceStartup + hostHeartbeatFrequency;

            await LobbyService.Instance.SendHeartbeatPingAsync(activeLobby.Id);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    #endregion

    public async Task<Lobby> CreateLobby(string relayJoinCode)
    {
        try
        {
            string hostName = AuthManager.PlayerName;
            string lobbyName = hostName + "'s host";
            isHost = true;
            isGameStarted = true;
            isPlayerReady = false;

            var options = new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Data = new Dictionary<string, DataObject>()
            {
                { keyLobbyHost, new DataObject(DataObject.VisibilityOptions.Public, hostName) },
                { keyLobbyRelayJoinCode, new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode) }
            };

            options.Player = CreatePlayerData();

            activeLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, options);

            inLobbyPlayers = activeLobby?.Players;
            Debug.Log($"lobby created, lobby data:{activeLobby}");
        }
        catch (Exception e)
        {
            Debug.Log($"lobby creation failed, e:{e.Message}");
            activeLobby = null;
        }

        return activeLobby;
    }

    public async Task<Lobby> JoinLobby(string lobbyId)
    {
        try
        {
            await PrepareJoinLobby();

            var options = new JoinLobbyByIdOptions
            {
                Player = CreatePlayerData()
            };

            activeLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);

            inLobbyPlayers = activeLobby?.Players;
            Debug.Log($"joined lobby, lobby data:{activeLobby}");
        }
        catch (Exception e)
        {
            Debug.Log($"joining lobby failed, e:{e.Message}");
            activeLobby = null;
        }

        return activeLobby;
    }

    public async void DeleteLobby()
    {
        try
        {
            if (activeLobby == null) return;

            await LobbyService.Instance.DeleteLobbyAsync(activeLobby.Id);
            Debug.Log($"lobby deleted");
        }
        catch (Exception e)
        {
            Debug.Log($"delete lobby failed, e:{e.Message}");
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            if (activeLobby == null) return;

            string playerId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(activeLobby.Id, playerId);
            Debug.Log($"lobby left");
        }
        catch (Exception e)
        {
            Debug.Log($"leave lobby failed, e:{e.Message}");
        }
    }

    public async Task<List<Lobby>> RetrieveLobbyLists()
    {
        QueryLobbiesOptions options = new QueryLobbiesOptions
        {
            Count = maxLobbiesToList,
            // order by newest first
            Order = new List<QueryOrder>
            {
                new(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created
                )
            },
            Filters = new List<QueryFilter>
            {
                new(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                )
            }
        };

        var result = await LobbyService.Instance.QueryLobbiesAsync(options);

        Debug.Log(
            $"retrieved lobbies: count:{result.Results.Count}, names:{string.Join(",", result.Results.Select(l => l.Name))}");

        return result.Results;
    }

    public async void QuickJoinLobby()
    {
        if (activeLobby != null) return;

        try
        {
            if (activeLobby == null) return;

            activeLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log($"Quickly joined to lobby, lobby data:{activeLobby}");
        }
        catch (Exception e)
        {
            Debug.Log($"unable to quick join, e:{e.Message}");
            activeLobby = null;
        }
    }

    private async Task PrepareJoinLobby()
    {
        isHost = false;
        isGameStarted = false;
        isPlayerReady = false;

        if (activeLobby != null)
        {
            Debug.Log($"player is in a lobby while trying to connect new lobby, leaving old lobby");
            await LeaveJoinedLobby();
        }
    }

    public async Task LeaveJoinedLobby()
    {
        try
        {
            await RemovePlayer(AuthManager.PlayerId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async Task RemovePlayer(string playerId)
    {
        try
        {
            if (activeLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(activeLobby.Id, playerId);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    #region Player

    private Player CreatePlayerData()
    {
        var player = new Player
        {
            Data = CreatePlayerDictionary()
        };

        return player;
    }

    private Dictionary<string, PlayerDataObject> CreatePlayerDictionary()
    {
        var playerDictionary = new Dictionary<string, PlayerDataObject>
        {
            { keyPlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, AuthManager.PlayerName) },
            {
                keyPlayerReady,
                new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isPlayerReady.ToString())
            },
        };

        return playerDictionary;
    }

    #endregion
}