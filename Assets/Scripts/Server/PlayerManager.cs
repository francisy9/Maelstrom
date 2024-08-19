using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkManager
{
    [SerializeField] private GameManager gameManager;
    private int numPlayersConnected;

    public override void OnStartServer() {
        autoCreatePlayer = false;
        gameManager.gameObject.SetActive(true);
        return;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject playerObject = gameManager.GetPlayer().gameObject;
        NetworkServer.AddPlayerForConnection(conn, playerObject);
        Debug.Log("PlayerManager: connection added as player");
        numPlayersConnected += 1;

        if (numPlayersConnected == 2) {
            gameManager.StartGame();
        }
    }
}
