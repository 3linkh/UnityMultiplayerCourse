using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField]
    Transform leaderBoardEntityHolder;

    [SerializeField]
    LeaderboardEntityDisplay leaderboardEntityPrefab;

    [SerializeField]
    int entitiesToDisplay = 8;

    private NetworkList<LeaderBoardEntityState> leaderBoardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();

    private void Awake()
    {
        leaderBoardEntities = new NetworkList<LeaderBoardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderBoardEntities.OnListChanged += HandleLeaderBoardEntitiesChanged;
            foreach (LeaderBoardEntityState entity in leaderBoardEntities)
            {
                HandleLeaderBoardEntitiesChanged(
                    new NetworkListEvent<LeaderBoardEntityState>
                    {
                        Type = NetworkListEvent<LeaderBoardEntityState>.EventType.Add,
                        Value = entity,
                    }
                );
            }
        }

        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach (TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderBoardEntities.OnListChanged -= HandleLeaderBoardEntitiesChanged;
        }
        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderBoardEntitiesChanged(
        NetworkListEvent<LeaderBoardEntityState> changeEvent
    )
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Add:
                if (!entityDisplays.Any(x => x.ClientID == changeEvent.Value.ClientId))
                {
                    LeaderboardEntityDisplay leaderboardEntity = Instantiate(
                        leaderboardEntityPrefab,
                        leaderBoardEntityHolder
                    );
                    leaderboardEntity.Initialise(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Coins
                    );
                    entityDisplays.Add(leaderboardEntity);
                }

                break;
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Remove:
                LeaderboardEntityDisplay displayToRemove = entityDisplays.FirstOrDefault(x =>
                    x.ClientID == changeEvent.Value.ClientId
                );
                if (displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }
                break;
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate = entityDisplays.FirstOrDefault(x =>
                    x.ClientID == changeEvent.Value.ClientId
                );
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            bool shouldShow = i <= entitiesToDisplay - 1;
            entityDisplays[i].gameObject.SetActive(shouldShow);
        }

        LeaderboardEntityDisplay mydisplay = entityDisplays.FirstOrDefault(x =>
            x.ClientID == NetworkManager.Singleton.LocalClientId
        );

        if (mydisplay != null)
        {
            if (mydisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderBoardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                mydisplay.gameObject.SetActive(true);
            }
        }
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        leaderBoardEntities.Add(
            new LeaderBoardEntityState
            {
                ClientId = player.OwnerClientId,
                PlayerName = player.playerName.Value,
                Coins = 0,
            }
        );

        player.wallet.totalCoins.OnValueChanged += (oldCoins, newCoins) =>
            HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (leaderBoardEntities == null)
            return;
        foreach (LeaderBoardEntityState entity in leaderBoardEntities)
        {
            if (entity.ClientId != player.OwnerClientId)
            {
                continue;
            }

            leaderBoardEntities.Remove(entity);
            break;
        }
        player.wallet.totalCoins.OnValueChanged -= (oldCoins, newCoins) =>
            HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleCoinsChanged(ulong clientID, int newCoins)
    {
        for (int i = 0; i < leaderBoardEntities.Count; i++)
        {
            if (leaderBoardEntities[i].ClientId != clientID)
                continue;
            leaderBoardEntities[i] = new LeaderBoardEntityState
            {
                ClientId = leaderBoardEntities[i].ClientId,
                PlayerName = leaderBoardEntities[i].PlayerName,
                Coins = newCoins,
            };
            return;
        }
    }
}
