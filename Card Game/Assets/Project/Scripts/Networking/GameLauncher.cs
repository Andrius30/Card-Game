using Fusion;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] Button connectBtn;

    FusionCallbacks.ConnectionStatus _status = FusionCallbacks.ConnectionStatus.Disconnected;

    FusionCallbacks fusionCallbacks;

    void Start()
    {
        connectBtn.onClick.AddListener(Connect);
        fusionCallbacks = GetComponent<FusionCallbacks>();
    }

    void Connect()
    {
        connectBtn.gameObject.SetActive(false);
        LevelManager levelManager = GetComponent<LevelManager>();
        fusionCallbacks.Launch(levelManager, OnConnectionStatusUpdate);
    }

    void OnConnectionStatusUpdate(NetworkRunner runner, FusionCallbacks.ConnectionStatus status, string reason)
    {
        if (!this)
            return;

        if (status != _status)
        {
            switch (status)
            {
                case FusionCallbacks.ConnectionStatus.Disconnected:
                    ErrorBox.onError.Invoke($"Disconnected! {reason}");
                    break;
                case FusionCallbacks.ConnectionStatus.Failed:
                    ErrorBox.onError.Invoke($"Error! {reason}");
                    break;
            }
        }

        _status = status;
    }

}
