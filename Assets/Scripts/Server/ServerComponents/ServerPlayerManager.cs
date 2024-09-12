using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ServerPlayerManager : NetworkBehaviour
{
    [SerializeField] private Player playerOne;
    [SerializeField] private Player playerTwo;
    private bool p1Assigned = false;
    private bool p2Assigned = false;

    [Server]
    // Used for initialization of game by player manager
    public Player GetPlayerRefBeforeStart() {
        if (!p1Assigned) {
            p1Assigned = true;
            return playerOne;
        } else if (!p2Assigned) {
            p2Assigned = true;
            return playerTwo;
        } else {
            Debug.LogError("Too many attempted connections");
            return null;
        }
    }

    [Server]
    public Player GetPlayerOne() => playerOne;
    [Server]
    public Player GetPlayerTwo() => playerTwo;
    [Server]
    public Player GetOpposingPlayer(Player player) {
        return player == playerOne ? playerTwo : playerOne;
    }
    [Server]
    public Player GetInTurnPlayer() {
        return GameManager.Instance.IsP1Turn() ? playerOne : playerTwo;
    }
    [Server]
    public Player GetNextPlayer() {
        return GameManager.Instance.IsP1Turn() ? playerTwo : playerOne;
    }
}
