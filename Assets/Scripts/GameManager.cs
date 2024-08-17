using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using static Types;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player playerOne;
    // TODO: @max figure out how to load this in on run time based on user
    [SerializeField] private List<CardStatsSO> p1Deck;
    private bool p1Assigned = false;
    [SerializeField] private Player playerTwo;
    [SerializeField] private List<CardStatsSO> p2Deck;
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
        ServerUpdate.Instance.SetPlayerOneRef(playerOne);

        Player starter = GetInTurnPlayer();
        Player nextPlayer = GetNextPlayer();
        int numCardsToBeDrawnByFirstPlayer = 3;
        int numCardsToBeDrawnBySecondPlayer = 4;

        for (int i = 0; i < numCardsToBeDrawnByFirstPlayer; i++) {
            CardStatsSO cardDrawn = DrawCardStatSO(starter);
            starter.AddCardToHand(cardDrawn);
            nextPlayer.AddCardToOpponentHand();
            ServerUpdate.Instance.AddCardToHand(cardDrawn, starter);
        }

        for (int j = 0; j < numCardsToBeDrawnBySecondPlayer; j++) {
            CardStatsSO cardDrawn = DrawCardStatSO(nextPlayer);
            nextPlayer.AddCardToHand(cardDrawn);
            starter.AddCardToOpponentHand();
            ServerUpdate.Instance.AddCardToHand(cardDrawn, nextPlayer);
        }

        InitializeManaDisplays();
        starter.IncrementMaxMana();


        // TODO: testing, remove when done
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();



        starter.RefreshMana();

        starter.TargetStartGame(true);
        nextPlayer.TargetStartGame(false);
        OnGameStart?.Invoke(this, EventArgs.Empty);
        StartTurn();
        ServerUpdate.Instance.PrintServerGameState();
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

    private bool IsPlayerInTurn(Player requestingPlayer) {
        return GetInTurnPlayer() == requestingPlayer;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn(NetworkConnectionToClient sender = null) {
        Debug.Log($"end turn called, sender: {sender.identity}");
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        
        if (IsPlayerInTurn(requestingPlayer)) {
            requestingPlayer.TargetEndTurn();
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

    [Command(requiresAuthority = false)]
    public void CmdPlayCard(int handIndex, CardStatsSO cardStatsSO, int boardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        Debug.Log($"{requestingPlayer.name} is requesting to play card");
        
        if(IsPlayerInTurn(requestingPlayer)) {
            CardStatsSO cardToBePlayed = ServerUpdate.Instance.GetCardStatSoAtHandIndex(handIndex, requestingPlayer);
            if (!cardToBePlayed.EqualValues(cardStatsSO)) {
                Debug.LogError("Server mismatch");
            }
            if (requestingPlayer.GetMana() < cardToBePlayed.manaCost) {
                Debug.LogError("Insufficient mana to play card but still able to make request");
            }

            Debug.Log($"{requestingPlayer.name} successfully played card {cardStatsSO.cardName} mana cost: {cardStatsSO.manaCost}");
            
            InPlayStats inPlayStats = OnPlay.Instance.GetInPlayStatsFromCardStatsSO(cardStatsSO);
            requestingPlayer.ConsumeMana(cardToBePlayed.manaCost);
            requestingPlayer.TargetPlayCard(inPlayStats, boardIndex);
            GetNextPlayer().TargetOpponentPlayCard(cardToBePlayed, inPlayStats);
            ServerUpdate.Instance.MoveCardToBoard(requestingPlayer, handIndex, inPlayStats, boardIndex);

            ServerUpdate.Instance.PrintServerGameState();
        } else {
            Debug.LogError("Player isn't in turn");
        }
    }

    [Server]
    private CardStatsSO DrawCardStatSO(Player player) {
        List<CardStatsSO> deckToDrawFrom = player == playerOne ? p1Deck : p2Deck;
        int cardIndex = UnityEngine.Random.Range(0, deckToDrawFrom.Count);
        CardStatsSO cardDrawn = deckToDrawFrom[cardIndex];
        deckToDrawFrom.RemoveAt(cardIndex);
        return cardDrawn;
    }

    // Server functions for debugging purposes
    public int GetP1Health() {
        return playerOne.GetHp();
    }

    public int GetP2Health() {
        return playerTwo.GetHp();
    }

    public int GetP1Mana() {
        return playerOne.GetMana();
    }

    public int GetP2Mana() {
        return playerTwo.GetMana();
    }

    public int GetP1MaxMana() {
        return playerOne.GetTotalMana();
    }

    public int GetP2MaxMana() {
        return playerTwo.GetTotalMana();
    }

    public Player GetPlayerOne() {
        return playerOne;
    }

    public Player GetPlayerTwo() {
        return playerTwo;
    }
    // End of server debug functions
}
