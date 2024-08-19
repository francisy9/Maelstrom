using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static Types;
using System.Text;

public class ServerUpdate : NetworkBehaviour
{
    public static ServerUpdate Instance;
    private List<CardStats> p1Hand;
    private List<CardStats> p1Board;
    private List<CardStats> p2Hand;
    private List<CardStats> p2Board;
    private GameManager gameManager;
    private Player playerOne;
    private Player playerTwo;

    public override void OnStartServer() {
        Instance = this;
        p1Hand = new List<CardStats>();
        p1Board = new List<CardStats>();
        p2Hand = new List<CardStats>();
        p2Board = new List<CardStats>();
        gameManager = GameManager.Instance;
        Debug.Log("Server Update Instance created");
    }

    [Server]
    public void SetPlayerRefs(Player playerOne, Player playerTwo) {
        this.playerOne = playerOne;
        this.playerTwo = playerTwo;
    }

    [Server]
    public void AddCardToHand(CardStatsSO cardStatsSO, Player player) {
        CardStats cardStats = new CardStats(cardStatsSO);
        if (player == playerOne) {
            p1Hand.Add(cardStats);
        } else {
            p2Hand.Add(cardStats);
        }
    }

    [Server]
    public void MoveCardToBoard(Player player, int handIndex, CardStats cardStats, int boardIndex) {
        Debug.Log(boardIndex);
        Debug.Log($"p1 board size: {p1Board.Count} p2 board size: {p2Board.Count}");
        Debug.Log(handIndex);
        Debug.Log($"p1 hand size: {p1Hand.Count} p2 hand size: {p2Hand.Count}");
        if (player == playerOne) {
            p1Board.Insert(boardIndex, cardStats);
            p1Hand.RemoveAt(handIndex);
        } else {
            p2Board.Insert(boardIndex, cardStats);
            p2Hand.RemoveAt(handIndex);
        }
    }

    [Server]
    public void PrintServerGameState() {
        Debug.Log($"{GameManager.Instance.GetInTurnPlayer().name}'s turn " +
        $"p1: {gameManager.GetP1Health()}hp {gameManager.GetP1Mana()}/{gameManager.GetP1MaxMana()} mana" +
        $" p2: {gameManager.GetP2Health()}hp {gameManager.GetP2Mana()}/{gameManager.GetP2MaxMana()} mana");

        StringBuilder p2HandText = new StringBuilder();
        p2HandText.Append("p2 hand: ");
        foreach (CardStats cardStats in p2Hand) {
            p2HandText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p2HandText);

        StringBuilder p2BoardText = new StringBuilder();
        p2BoardText.Append("p2 board: ");
        foreach (CardStats cardStats in p2Board) {
            p2BoardText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p2BoardText);

        StringBuilder p1BoardText = new StringBuilder();
        p1BoardText.Append("p1 board: ");
        foreach (CardStats cardStats in p1Board) {
            p1BoardText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p1BoardText);

        StringBuilder p1HandText = new StringBuilder();
        p1HandText.Append("p1 hand: ");
        foreach (CardStats cardStats in p1Hand) {
            p1HandText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p1HandText);
    }

    [Server]
    public CardStats GetCardStatsAtHandIndex(int i, Player player) {
        return player == playerOne ? p1Hand[i] : p2Hand[i];
    }

    [Server]
    public CardStats GetCardStatsAtBoardIndex(int i, Player player) {
        return player == playerOne ? p1Board[i] : p2Board[i];
    }

    private string GetCardInfoString(CardStats cardStats) {
        return $"{cardStats.CardName} mana: {cardStats.CurrentManaCost} hp: {cardStats.CurrentHP}/{cardStats.MaxHP}";
    }

    [Server]
    public CardStats[] Attack(int boardIndex, int opponentBoardIndex, Player player) {
        CardStats card;
        CardStats opponentCard;
        List<CardStats> board;
        List<CardStats> opponentBoard;
        if (player == playerOne) {
            card = GetCardStatsAtBoardIndex(boardIndex, playerOne);
            opponentCard = GetCardStatsAtBoardIndex(opponentBoardIndex, playerTwo);
            board = p1Board;
            opponentBoard = p2Board;
        } else {
            card = GetCardStatsAtBoardIndex(boardIndex, playerTwo);
            opponentCard = GetCardStatsAtBoardIndex(opponentBoardIndex, playerOne);
            board = p2Board;
            opponentBoard = p1Board;
        }

        card.NumAttacks -= 1;
        card.CurrentHP -= opponentCard.CurrentAttack;

        opponentCard.CurrentHP -= card.CurrentAttack;

        if (card.CurrentHP <= 0) {
            board.RemoveAt(boardIndex);
        }

        if (opponentCard. CurrentHP <= 0) {
            opponentBoard.RemoveAt(opponentBoardIndex);
        }
        
        return new CardStats[] {
            card, opponentCard
        };
    }

    [Server]
    public void EndTurn(Player player) {
        List<CardStats> board = p1Board;
        if (player == playerTwo) {
            board = p2Board;
        }

        foreach (CardStats cardStats in board) {
            cardStats.NumAttacks = cardStats.TotalNumAttacks;
        }
    }
}
