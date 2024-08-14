using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player playerOne;
    private bool p1Assigned = false;
    [SerializeField] private Player playerTwo;
    private bool p2Assigned = false;
    [SerializeField] private Button endTurnButton;
    public event EventHandler OnGameStart;

    private int turn;
    private bool isP1Turn;

    public override void OnStartServer() {
        Instance = this;
        isP1Turn = UnityEngine.Random.Range(0, 2) == 0;

        // TODO: Testing logic, remove when done
        isP1Turn = true;
        turn = 0;
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isServer) return;

        Instance = this;
    }

    // Used for initialization of game by player manager
    public Player GetPlayer() {
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

    public void StartGame() {
        Debug.Log("start game");

        Player starter = GetInTurnPlayer();
        Player nextPlayer = GetNextPlayer();

        starter.DrawCards(3);
        nextPlayer.DrawCards(4);
        InitializeManaDisplays();
        starter.IncrementMaxMana();
        starter.RefreshMana();

        Debug.Log($"Player one mana: {playerOne.GetMana()}/{playerOne.GetTotalMana()}");

        starter.TargetStartGame(true);
        nextPlayer.TargetStartGame(false);
        OnGameStart?.Invoke(this, EventArgs.Empty);
        StartTurn();
    }

    private void InitializeManaDisplays() {
        playerOne.TargetInitializeManaDisplay();
        playerTwo.TargetInitializeManaDisplay();
    }

    private void StartTurn() {
        GetInTurnPlayer().TargetStartTurn();
        GetNextPlayer().TargetStartOpponentTurn();
    }

    public bool IsP1Turn() {
        return isP1Turn;
    }

    public Player GetInTurnPlayer() {
        return isP1Turn ? playerOne : playerTwo;
    }

    public Player GetNextPlayer() {
        return isP1Turn ? playerTwo : playerOne;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn(NetworkConnectionToClient sender = null) {
        Debug.Log($"end turn called, sender: {sender.identity}");
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        Player currentInTurnPlayer = GetInTurnPlayer();
        if (currentInTurnPlayer == requestingPlayer) {
            currentInTurnPlayer.TargetEndTurn();
            isP1Turn = !isP1Turn;
            Player playerNextInTurn = GetInTurnPlayer();
            playerNextInTurn.IncrementMaxMana();
            playerNextInTurn.RefreshMana();
            playerNextInTurn.TargetStartTurn();
            turn += 1;
        } else {
            Debug.LogError("Wrong player was able to request end turn");
        }
    }
}
