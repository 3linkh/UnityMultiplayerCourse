using System;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    CinemachineCamera virtualCamera;

    [field: SerializeField]
    public Health Health { get; private set; }

    [field: SerializeField]
    public CoinWallet wallet { get; private set; }

    [Header("Settings")]
    [SerializeField]
    int ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> playerName =
        new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.HostGameManager.NetworkServer.GetUserDataByClientId(
                    OwnerClientId
                );

            playerName.Value = userData.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
