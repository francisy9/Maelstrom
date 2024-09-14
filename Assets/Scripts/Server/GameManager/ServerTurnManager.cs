using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ServerTurnManager : NetworkBehaviour
{
    [SyncVar] private int turn;
    [SyncVar] private bool isP1Turn;
    public event EventHandler OnGameStart;

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
    public void StartGame() {
        Debug.Log("start game");
        ServerState.Instance.SetPlayerRefs(GameManager.Instance.GetPlayerOne(), GameManager.Instance.GetPlayerTwo());

        Player starter = GameManager.Instance.GetInTurnPlayer();
        Player nextPlayer = GameManager.Instance.GetPlayerManager().GetNextPlayer();
        GameManager.Instance.GetHandManager().StartGameDrawCards();

        GameManager.Instance.GetManaManager().InitializeManaDisplays();
        GameManager.Instance.GetManaManager().IncrementAndRefreshPlayerMaxMana(starter);

        GameManager.Instance.GetHeroManager().InitializeHeroes();
        starter.TargetBeginGame(true);
        nextPlayer.TargetBeginGame(false);

        OnGameStart?.Invoke(this, EventArgs.Empty);
        StartTurn();
        ServerState.Instance.PrintServerGameState();
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
