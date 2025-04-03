using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }


    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public class OnClickedOnGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }
    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayablePlayerTypeChange;

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one GameManager instance!");
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerOnClientConnectedCallback;
        }

        currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayablePlayerTypeChange?.Invoke(this, EventArgs.Empty);
        };
    }

    private void NetworkManagerOnClientConnectedCallback(ulong obj)
    {
        if(NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            // Start Game
            currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        Debug.Log("ClickedOnGridPosition " + x + ", " + y);
        if (playerType != currentPlayablePlayerType.Value)
        { return; }
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType
        });

        switch(currentPlayablePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
    }


    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType()
    {
        return currentPlayablePlayerType.Value;
    }
}
