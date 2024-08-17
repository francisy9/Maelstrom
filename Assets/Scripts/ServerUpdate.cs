using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static Types;
using System.Text;

public class ServerUpdate : NetworkBehaviour
{
    public static ServerUpdate Instance;
    private List<CardStatsSO> p1Hand;
    private List<(CardStatsSO, InPlayStats)> p1Board;
    private List<CardStatsSO> p2Hand;
    private List<(CardStatsSO, InPlayStats)> p2Board;
    private GameManager gameManager;
    private Player playerOne;

    public override void OnStartServer() {
        Instance = this;
        p1Hand = new List<CardStatsSO>();
        p1Board = new List<(CardStatsSO, InPlayStats)>();
        p2Hand = new List<CardStatsSO>();
        p2Board = new List<(CardStatsSO, InPlayStats)>();
        gameManager = GameManager.Instance;
        Debug.Log("Server Update Instance created");
    }

    [Server]
    public void SetPlayerOneRef(Player player) {
        playerOne = player;
    }

    [Server]
    public void AddCardToP1Hand(CardStatsSO cardStatsSO) {
        p1Hand.Add(cardStatsSO);
    }

    [Server]
    public void AddCardToP2Hand(CardStatsSO cardStatsSO) {
        p2Hand.Add(cardStatsSO);
    }

    [Server]
    public void AddCardToHand(CardStatsSO cardStatsSO, Player player) {
        if (player == playerOne) {
            p1Hand.Add(cardStatsSO);
        } else {
            p2Hand.Add(cardStatsSO);
        }
    }

    [Server]
    public void MoveCardToBoard(Player player, int handIndex, InPlayStats inPlayStats, int boardIndex) {
        Debug.Log(boardIndex);
        Debug.Log($"p1 board size: {p1Board.Count} p2 board size: {p2Board.Count}");
        Debug.Log(handIndex);
        Debug.Log($"p1 hand size: {p1Hand.Count} p2 hand size: {p2Hand.Count}");
        if (player == playerOne) {
            p1Board.Insert(boardIndex, (p1Hand[handIndex], inPlayStats));
            p1Hand.RemoveAt(handIndex);
        } else {
            p2Board.Insert(boardIndex, (p2Hand[handIndex], inPlayStats));
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
        foreach (CardStatsSO cardStatsSO in p2Hand) {
            p2HandText.Append(GetCardInfoString(cardStatsSO) + " ");
        }
        Debug.Log(p2HandText);

        StringBuilder p2BoardText = new StringBuilder();
        p2BoardText.Append("p2 board: ");
        foreach ((CardStatsSO, InPlayStats) tup in p2Board) {
            p2BoardText.Append(GetOnBoardCardInfoString(tup) + " ");
        }
        Debug.Log(p2BoardText);

        StringBuilder p1BoardText = new StringBuilder();
        p1BoardText.Append("p1 board: ");
        foreach ((CardStatsSO, InPlayStats) tup in p1Board) {
            p1BoardText.Append(GetOnBoardCardInfoString(tup) + " ");
        }
        Debug.Log(p1BoardText);

        StringBuilder p1HandText = new StringBuilder();
        p1HandText.Append("p1 hand: ");
        foreach (CardStatsSO cardStatsSO in p1Hand) {
            p1HandText.Append(GetCardInfoString(cardStatsSO) + " ");
        }
        Debug.Log(p1HandText);
    }

    [Server]
    public CardStatsSO GetCardStatSoAtHandIndex(int i, Player player) {
        return player == playerOne ? p1Hand[i] : p2Hand[i];
    }

    private string GetCardInfoString(CardStatsSO cardStatsSO) {
        return $"{cardStatsSO.cardName} mana: {cardStatsSO.manaCost}";
    } 

    private string GetOnBoardCardInfoString((CardStatsSO, InPlayStats) stats) {
        return $"{stats.Item1.cardName} mana: {stats.Item1.manaCost} hp: {stats.Item2.hp} atk: {stats.Item2.attackVal}";
    } 
}
