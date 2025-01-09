using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using QFSW.QC;

public class TestLobby : MonoBehaviour {
    async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
 
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    [Command]
    private async void CreateLobby() {
        try {
            string lobbyName = "some lobby";
            int numPlayes = 2;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, numPlayes);

            Debug.Log("Created lobby " + lobbyName + "with " + numPlayes + " players");
        } catch (LobbyServiceException e) {
            Debug.Log("Failed to create lobby: " + e);
        }
    }

    [Command]
    private async void QueryLobbies() {
        QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync();

        Debug.Log("Lobbies found: " + lobbies.Results);
        Debug.Log(lobbies.Results.Count + " lobbies");
    }
}
