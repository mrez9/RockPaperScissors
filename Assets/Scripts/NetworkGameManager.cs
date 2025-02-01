using System;
using System.Collections;
using UnityEngine;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System.Collections.Generic;
using FishNet.Transporting;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance;

    [SerializeField]
    private string hostIP = "localhost";
    public bool _isHost;

    private List<NetworkConnection> _connectedClients = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        // Subscribe to connection events
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionStateChanged;
    }

    private void OnDestroy()
    {
        // Unsubscribe from connection events
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionStateChanged;
    }

    // Called when a client connection state changed via remote serevr
    private void OnRemoteConnectionStateChanged(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            Debug.Log($"new client connected,  {DateTime.Now}");
            _connectedClients.Add(conn);
            CheckIfGameCanStart();
        }
        else if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            _connectedClients.Remove(conn);
        }
    }

    // Check if the number of connected clients is 2
    private void CheckIfGameCanStart()
    {
        if (_connectedClients.Count == 2)
        {
            StartCoroutine(StartGameWithDelay());
        }
    }

    private IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log($"Starting Game On Server");
        GameManager.instance.StartGame();
    }

    // Called from Host Button
    public void StartHost()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection(hostIP);
        _isHost = true;

        GameSceneUI.instance.ShowConnectionInfo(true);
    }

    // Called from Client Button
    public void ConnectToHost(string ip)
    {
        hostIP = ip;
        InstanceFinder.ClientManager.StartConnection(hostIP);

        GameSceneUI.instance.ShowConnectionInfo(false);
    }
}