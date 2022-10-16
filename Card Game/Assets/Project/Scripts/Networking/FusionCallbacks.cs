using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    Action<NetworkRunner, FusionCallbacks.ConnectionStatus, string> onConnect;

    NetworkRunner runner;
    LevelManager levelManager;
    ConnectionStatus status;

    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Failed,
        Connected,
        Loading,
        Loaded
    }

    public async void Launch(INetworkSceneManager sceneLoader, Action<NetworkRunner, FusionCallbacks.ConnectionStatus, string> onConnectionStatusupdated)
    {
        this.onConnect = onConnectionStatusupdated;

        if (levelManager == null)
            levelManager = GetComponent<LevelManager>();

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.name = name;
        runner.ProvideInput = false;

        SetConnectionStatus(ConnectionStatus.Connecting, "");

        // Start or join (depends on gamemode) a session with a specific name
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = null,
            PlayerCount = 2,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = sceneLoader
        });
        levelManager.LoadLevel(1); // lobby scene
    }
    public void SetConnectionStatus(ConnectionStatus status, string message)
    {
        this.status = status;
        if (onConnect != null)
            onConnect(runner, status, message);
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        SetConnectionStatus(status, "Player joined");
        if (runner.IsServer)
        {
            GameManager.instance.playersJoined++;
        }
        if(GameManager.instance.playersJoined == 2)
        {
            levelManager.LoadLevel(2);
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        SetConnectionStatus(status, "Player left");
        if (runner.IsServer)
        {
            GameManager.instance.playersJoined--;
        }
    }
    public void OnConnectedToServer(NetworkRunner runner) => SetConnectionStatus(ConnectionStatus.Connected, "");
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) => SetConnectionStatus(ConnectionStatus.Failed, reason.ToString());
    public void OnDisconnectedFromServer(NetworkRunner runner) => SetConnectionStatus(ConnectionStatus.Disconnected, "");
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) => SetConnectionStatus(ConnectionStatus.Disconnected, shutdownReason.ToString());
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) => request.Accept();

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }


}
