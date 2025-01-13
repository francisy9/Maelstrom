using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using QFSW.QC;
using System.Collections.Generic;

public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby;
    private float maxHeartBeatInterval = 15f;
    private float currentHeartBeatTimer = 0f;
    private string playerName;


    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "12.9 v" + Random.Range(0, 99) + "." + Random.Range(1,5);
        Debug.Log(playerName);
    }

    private void Update()
    {
        SendHeartBeat();
    }

    private async void SendHeartBeat()
    {
        if (hostLobby != null)
        {
            try
            {
                if (currentHeartBeatTimer > maxHeartBeatInterval)
                {
                    currentHeartBeatTimer = 0f;
                    await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                }
                else
                {
                    currentHeartBeatTimer += Time.deltaTime;
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("Failed to send heartbeat: " + e);
            }
        }
    }

    [Command]
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "some lobby";
            int numPlayes = 2;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(
                lobbyName,
                numPlayes,
                new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject> {
                        { "GameMode", new DataObject(
                            DataObject.VisibilityOptions.Public,
                            "Normal",
                            DataObject.IndexOptions.S1
                        )
                        },
                    }
                });
            hostLobby = lobby;
            Debug.Log("Created lobby " + lobbyName + "with " + numPlayes + " players");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Failed to create lobby: " + e);
        }
    }

    [Command]
    private async void QueryLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter> {
                    new QueryFilter(
                        QueryFilter.FieldOptions.AvailableSlots,
                        "0",
                        QueryFilter.OpOptions.GT),
                    new QueryFilter(
                        QueryFilter.FieldOptions.S1,
                        "Normal",
                        QueryFilter.OpOptions.EQ
                    ),
                },
            };
            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            Debug.Log("Lobbies found: " + lobbies.Results);
            Debug.Log(lobbies.Results.Count + " lobbies");

            foreach (Lobby lobby in lobbies.Results)
            {
                Debug.Log("Lobby name: " + lobby.Name);
                Debug.Log("Lobby id: " + lobby.Id);
                PrintPlayers(lobby.Players);
                Debug.Log("Lobby max players: " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Failed to query lobbies: " + e);
        }
    }

    private void PrintPlayers(List<Unity.Services.Lobbies.Models.Player> players) {
        foreach(Unity.Services.Lobbies.Models.Player player in players) {
            foreach(KeyValuePair<string, PlayerDataObject> kvp in player.Data) {
                Debug.Log($"Key = {kvp.Key}, Value = {kvp.Value.Value}");
            }
        }
    }

    [Command]
    private async void QuickJoin()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Failed to quick join lobby" + e);
        }
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player{
            Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        };
    }
}