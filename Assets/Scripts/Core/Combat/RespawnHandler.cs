using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField]
    private TankPlayer playerPrefab;

    [SerializeField]
    private float keptCoinPercent = 0.5f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            return;

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(TankPlayer player)
    {
        int keptCoins = (int)(player.wallet.totalCoins.Value * (keptCoinPercent / 100f));

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID, int keptCoins)
    {
        yield return null;

        TankPlayer playerInstance = Instantiate(
            playerPrefab,
            SpawnPoint.GetRandomSpawnPos(),
            Quaternion.identity
        );

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
        playerInstance.wallet.totalCoins.Value += keptCoins;
    }
}
