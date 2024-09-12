using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ServerTurnManager : NetworkBehaviour
{
    [SyncVar] private int turn;
    [SyncVar] private bool isP1Turn;

    [Server]
    public void EndTurn(Player requestingPlayer) {
        ServerState.Instance.EndTurn(requestingPlayer);
        requestingPlayer.TargetEndTurn();
        isP1Turn = !isP1Turn;
        Player playerNextInTurn = GameManager.Instance.GetPlayerManager().GetInTurnPlayer();
        GameManager.Instance.GetManaManager().IncrementAndRefreshPlayerMaxMana(playerNextInTurn);
        playerNextInTurn.TargetStartTurn();
        turn += 1;
    }

    public void Initialize() {
        isP1Turn = UnityEngine.Random.Range(0, 2) == 0;
        turn = 0;
    }


    [Server]
    public void StartTurn() {
        GameManager.Instance.GetPlayerManager().GetInTurnPlayer().TargetStartTurn();
        GameManager.Instance.GetPlayerManager().GetNextPlayer().TargetStartOpponentTurn();
    }

    public void EndTurn() {
        isP1Turn = !isP1Turn;
        turn += 1;
    } 

    public bool IsPlayerInTurn(Player player) => player == GameManager.Instance.GetPlayerManager().GetInTurnPlayer();
    public bool IsP1Turn() => isP1Turn;
    public int GetTurn() => turn;
}
