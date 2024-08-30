using UnityEngine;
using Mirror;

public class PlayerManager : NetworkManager
{
    [SerializeField] private GameManager gameManager;
    private int numPlayersConnected;
    [SerializeField] private bool isTesting;

    public override void OnStartServer() {
        autoCreatePlayer = false;
        gameManager.gameObject.SetActive(true);
        if (isTesting) {
            gameManager.TestingCards();
        }
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
