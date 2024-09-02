using System.Collections.Generic;
using Mirror;
using UnityEngine;
using static Types;
using System.Text;
using System;

public class ServerUpdate : NetworkBehaviour
{
    public static ServerUpdate Instance;
    private List<BaseCard> p1Hand;
    private List<UnitCardStats> p1Board;
    private List<BaseCard> p2Hand;
    private List<UnitCardStats> p2Board;
    private GameManager gameManager;
    private Player playerOne;
    private HeroStats p1Hero;
    private Player playerTwo;
    private HeroStats p2Hero;

    public override void OnStartServer() {
        Instance = this;
        p1Hand = new List<BaseCard>();
        p1Board = new List<UnitCardStats>();
        p2Hand = new List<BaseCard>();
        p2Board = new List<UnitCardStats>();
        gameManager = GameManager.Instance;
        Debug.Log("Server Update Instance created");
    }

    [Server]
    public void SetPlayerRefs(Player playerOne, Player playerTwo) {
        this.playerOne = playerOne;
        this.playerTwo = playerTwo;
    }

    [Server]
    public BaseCard GetBaseCardFromSO(BaseCardSO baseCardSO) {
        switch (baseCardSO.cardType)
        {
            case CardType.Unit:
                UnitCardStats cardStats = new UnitCardStats(baseCardSO as UnitCardStatsSO);
                return cardStats;
            default:
                Debug.LogError("unimplemented card type");
                break;
        }
        return new BaseCard();
    }

    [Server]
    public void AddCardToHand(BaseCard baseCard, Player player) {
        GetHand(player).Add(baseCard);
    }

    [Server]
    public void InitHeroes(int p1Hp, int p2Hp) {
        p1Hero = new HeroStats(p1Hp);
        p2Hero = new HeroStats(p2Hp);
    }

    [Server]
    public void MoveUnitCardToBoard(Player player, int handIndex, UnitCardStats cardStats, int boardIndex) {
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
        $"p1: {p1Hero.CurrentHP}hp {gameManager.GetP1Mana()}/{gameManager.GetP1MaxMana()} mana" +
        $" p2: {p2Hero.CurrentHP}hp {gameManager.GetP2Mana()}/{gameManager.GetP2MaxMana()} mana");

        StringBuilder p1HandText = new StringBuilder();
        p1HandText.Append("p1 hand: ");
        foreach (UnitCardStats cardStats in p1Hand) {
            p1HandText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p1HandText);

        StringBuilder p1BoardText = new StringBuilder();
        p1BoardText.Append("p1 board: ");
        foreach (UnitCardStats cardStats in p1Board) {
            p1BoardText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p1BoardText);


        StringBuilder p2HandText = new StringBuilder();
        p2HandText.Append("p2 hand: ");
        foreach (UnitCardStats cardStats in p2Hand) {
            p2HandText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p2HandText);

        StringBuilder p2BoardText = new StringBuilder();
        p2BoardText.Append("p2 board: ");
        foreach (UnitCardStats cardStats in p2Board) {
            p2BoardText.Append(GetCardInfoString(cardStats) + " ");
        }
        Debug.Log(p2BoardText);

    }

    [Server]
    public BaseCard GetCardStatsAtHandIndex(int i, Player player) {
        return player == playerOne ? p1Hand[i] : p2Hand[i];
    }

    [Server]
    public UnitCardStats GetCardStatsAtBoardIndex(int i, Player player) {
        return player == playerOne ? p1Board[i] : p2Board[i];
    }

    [Server]
    public HeroStats GetHeroStats(Player player) {
        return player == playerOne ? p1Hero : p2Hero;
    }

    private string GetCardInfoString(UnitCardStats cardStats) {
        return $"{cardStats.CardName} mana: {cardStats.CurrentManaCost} hp: {cardStats.CurrentHP}/{cardStats.MaxHP}";
    }

    [Server]
    public object[] Attack(int boardIndex, int opponentBoardIndex, Player player) {
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        if (attackerIsCard & targetIsCard) {
            UnitCardStats card;
            UnitCardStats opponentCard;
            List<UnitCardStats> board;
            List<UnitCardStats> opponentBoard;

            card = GetCardStatsAtBoardIndex(boardIndex, player);
            opponentCard = GetCardStatsAtBoardIndex(opponentBoardIndex, GetOpponentPlayer(player));
            board = GetBoard(player);
            opponentBoard = GetOpponentBoard(player);

            card.NumAttacks -= 1;
            card.CurrentHP -= opponentCard.CurrentAttack;

            opponentCard.CurrentHP -= card.CurrentAttack;

            if (card.CurrentHP <= 0) {
                board.RemoveAt(boardIndex);
            }

            if (opponentCard.CurrentHP <= 0) {
                opponentBoard.RemoveAt(opponentBoardIndex);
            }
            
            return new UnitCardStats[] {
                card, opponentCard
            };
        } else if (attackerIsCard & !targetIsCard) {
            UnitCardStats card;
            HeroStats opponentHeroStats = GetOpponentHeroStats(player);
            List<UnitCardStats> board = GetBoard(player);
            card = GetCardStatsAtBoardIndex(boardIndex, player);

            card.NumAttacks -= 1;
            card.CurrentHP -= opponentHeroStats.CurrentAttack;

            opponentHeroStats.CurrentHP -= card.CurrentAttack;

            if (card.CurrentHP <= 0) {
                board.RemoveAt(boardIndex);
            }

            return new object[] {
                card, opponentHeroStats
            };
        } else if (!attackerIsCard & targetIsCard) {
            HeroStats heroStats = GetHeroStats(player);
            UnitCardStats targetCard;
            List<UnitCardStats> opponentBoard;
            targetCard = GetCardStatsAtBoardIndex(opponentBoardIndex, GetOpponentPlayer(player));
            opponentBoard = GetOpponentBoard(player);

            heroStats.NumAttacks -= 1;
            heroStats.CurrentHP -= targetCard.CurrentAttack;

            targetCard.CurrentHP -= heroStats.CurrentAttack;

            if (targetCard.CurrentHP <= 0) {
                opponentBoard.RemoveAt(opponentBoardIndex);
            }

            return new object[] {
                heroStats, targetCard
            };
        } else {
            HeroStats heroStats = GetHeroStats(player);
            HeroStats opponentHeroStats = GetOpponentHeroStats(player);

            heroStats.NumAttacks -= 1;
            heroStats.CurrentHP -= opponentHeroStats.CurrentAttack;

            opponentHeroStats.CurrentHP -= heroStats.CurrentAttack;

            return new object[] {
                heroStats, opponentHeroStats
            };
        }
    }

    [Server]
    public void EndTurn(Player player) {
        List<UnitCardStats> board = p1Board;
        HeroStats heroStats = p1Hero;
        if (player == playerTwo) {
            board = p2Board;
            heroStats = p2Hero;
        }

        foreach (UnitCardStats cardStats in board) {
            cardStats.NumAttacks = cardStats.TotalNumAttacks;
        }
        heroStats.NumAttacks = heroStats.TotalNumAttacks;
    }

    private HeroStats GetOpponentHeroStats(Player player) {
        return player == playerOne ? p2Hero : p1Hero;
    }

    private List<BaseCard> GetHand(Player player) {
        return player == playerOne ? p1Hand : p2Hand;
    }

    private List<UnitCardStats> GetBoard(Player player) {
        return player == playerOne ? p1Board : p2Board;
    }

    private List<UnitCardStats> GetOpponentBoard(Player player) {
        return player == playerOne ? p2Board : p1Board;
    }

    private List<BaseCard> GetOpponentHand(Player player) {
        return player == playerOne ? p2Hand : p1Hand;
    }

    private Player GetOpponentPlayer(Player player) {
        return player == playerOne ? playerTwo : playerOne;
    }
}
