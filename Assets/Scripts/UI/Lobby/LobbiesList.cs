using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] Transform lobbyItemsParent;
    [SerializeField] LobbyItem lobbyItemPrefab;
    private bool isJoining = false;


    private bool isRefreshing = false;
    void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) return;
        isRefreshing = true;

        try
        {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions();
            queryOptions.Count = 25;

            queryOptions.Filters = new List<QueryFilter>()
            {
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0"),
                new QueryFilter(field: QueryFilter.FieldOptions.IsLocked, op: QueryFilter.OpOptions.EQ, value: "0")
            };
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);

            foreach (Transform child in lobbyItemsParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                Instantiate(lobbyItemPrefab, lobbyItemsParent).Initialize(this, lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to refresh lobbies list: {e.Message}");
        }

        
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;

        isJoining = true;

        try
        {
            
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
        }

        isJoining = false;
    }
}
