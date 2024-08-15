using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static Types;

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
    public void MoveP1CardToBoard(int index, InPlayStats inPlayStats) {
        p1Board.Add((p1Hand[index], inPlayStats));
        p1Hand.RemoveAt(index);
    }

    [Server]
    public void MoveP2CardToBoard(int index, InPlayStats inPlayStats) {
        p2Board.Add((p1Hand[index], inPlayStats));
        p2Hand.RemoveAt(index);
    }

    [Server]
    public void PrintServerGameState() {
        Debug.Log($"{GameManager.Instance.GetInTurnPlayer().name}'s turn\n");
        Debug.Log($"p1: {gameManager.GetP1Health()}hp {gameManager.GetP1Mana()}/{gameManager.GetP1MaxMana()} mana");
        Debug.Log($"p2: {gameManager.GetP2Health()}hp {gameManager.GetP2Mana()}/{gameManager.GetP2MaxMana()} mana");

        Debug.Log("p2 hand:");
        foreach (CardStatsSO cardStatsSO in p2Hand) {
            Debug.Log(GetCardInfoString(cardStatsSO));
        }

        Debug.Log(p2Board);
        Debug.Log(p1Board);

        Debug.Log("p1 hand");
        foreach (CardStatsSO cardStatsSO in p1Hand) {
            Debug.Log(GetCardInfoString(cardStatsSO));
        }
    }

    [Server]
    public CardStatsSO GetCardStatSoAtHandIndex(int i, Player player) {
        return player == playerOne ? p1Hand[i] : p2Hand[i];
    }

    private string GetCardInfoString(CardStatsSO cardStatsSO) {
        return $"{cardStatsSO.cardName} mana: {cardStatsSO.manaCost}";
    } 
}
