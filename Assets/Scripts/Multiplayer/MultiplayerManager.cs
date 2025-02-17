using System;
using System.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting.UTP;
using Unity.Networking.Transport;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    [SerializeField]
    private NetworkManager fishnetNetworkManager;
    [SerializeField]
    private SceneUI sceneUI;

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

    public async Task<string> InitializeHost()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            FishyUnityTransport transport =
                fishnetNetworkManager.TransportManager.GetTransport<FishyUnityTransport>();

            Debug.Log(
                $"allocation:ipv4:{allocation.RelayServer.IpV4}, port:{(ushort)allocation.RelayServer.Port}," +
                $" key:{allocation.Key}, data:{allocation.ConnectionData}, region:{allocation.Region}");

            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            Debug.Log($"connected to relay as host, joinCode:{joinCode}");

            // Start host
            if (fishnetNetworkManager.ServerManager.StartConnection()) // Server is successfully started.
            {
                fishnetNetworkManager.ClientManager.StartConnection();
            }

            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }

    public async Task InitializeClient(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            FishyUnityTransport transport =
                fishnetNetworkManager.TransportManager.GetTransport<FishyUnityTransport>();

            transport.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);

            fishnetNetworkManager.ClientManager.StartConnection();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}