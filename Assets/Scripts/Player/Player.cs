using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using CardTypes;

public class Player : NetworkBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Player opponent;
    [SerializeField] private Hero hero;
    [SerializeField] private EnemyHero enemyHero;
    [SerializeField] private Board board;
    [SerializeField] private EnemyBoard enemyBoard;
    [SerializeField] private EnemyHand enemyHand;
    [SerializeField] private List<UnitCardSO> deck;
    [SerializeField] private GameObject cardObject;
    [SerializeField] private HandController handController;
    [SerializeField] private GameObject cardBackObject;
    [SerializeField] private PlayerManaSystem manaManager;
    [SerializeField] private PlayerTurnManager turnManager;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        handController.SetPlayer(this);
        enemyHand.SetPlayer(opponent);
        board.SetPlayer(this);
        InitializeComponents();
    }

    private void InitializeComponents() {
        manaManager.Initialize(this);
        turnManager.Initialize(this);
    }



    [TargetRpc]
    public void TargetBeginGame(bool first, int heroStartHp, int enemyHeroStartHp) {
        hero.InitHero(heroStartHp);
        enemyHero.InitHero(enemyHeroStartHp);
    }

    public Board GetDropZone() {
        return board;
    }

    public Player GetOpponent() {
        return opponent;
    }

    // Turn logic
    public bool IsTurn() => turnManager.IsInTurn();
    public void TargetStartTurn() => turnManager.TargetStartTurn();
    public void TargetEndTurn() => turnManager.TargetEndTurn();
    public void TargetStartOpponentTurn() => turnManager.TargetStartOpponentTurn();


    // Mana logic
    public void TargetInitializeManaDisplay() => manaManager.TargetInitializeManaDisplay();
    public void IncrementMaxMana() => manaManager.IncrementMaxMana();
    public void ConsumeMana(int mana) => manaManager.ConsumeMana(mana);
    public void RefreshMana() => manaManager.RefreshMana();
    public int GetMana() => manaManager.GetMana();
    public int GetMaxMana() => manaManager.GetMaxMana();

    [TargetRpc]
    public void AddCardToHand(byte[] serializedCardArray) {
        BaseCard baseCard = BaseCard.Deserialize(serializedCardArray);
        handController.AddCardToHand(baseCard);
    }

    [TargetRpc]
    public void AddCardToOpponentHand() {
        enemyHand.AddCardToHand();
    }

    public Canvas GetCanvas() {
        return canvas;
    }


    [TargetRpc]
    public void TargetPlayCard(int handIndex, int boardIndex) {
        handController.PlayUnitCard(handIndex, boardIndex);
    }

    [TargetRpc]
    public void TargetOpponentPlayCard(byte[] cardData, int handIndex, int boardIndex) {
        enemyHand.PlayCard(cardData, handIndex, boardIndex);
    }

    [TargetRpc]
    public void TargetAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) {
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        object attackerStats;
        object targetStats;

        if (attackerIsCard) {
            attackerStats = UnitCardStats.Deserialize(cardData);
        } else {
            attackerStats = HeroStats.Deserialize(cardData);
        }

        if (targetIsCard) {
            targetStats = UnitCardStats.Deserialize(opponentCardData);
        } else {
            targetStats = HeroStats.Deserialize(opponentCardData);
        }

        board.CardAttack(boardIndex, opponentBoardIndex, attackerStats, targetStats);
    }

    [TargetRpc]
    public void TargetOpponentAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) {
        // Note the flip in boardIndex and opponentBoardIndex
        // attacker is the opponent
        // self unit is target
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        object targetStats;
        object attackerStats;

        if (targetIsCard) {
            targetStats = UnitCardStats.Deserialize(cardData);
        } else {
            targetStats = HeroStats.Deserialize(cardData);
        }

        if (attackerIsCard) {
            attackerStats = UnitCardStats.Deserialize(opponentCardData);
        } else {
            attackerStats = HeroStats.Deserialize(opponentCardData);
        }

        board.CardAttackedBy(boardIndex, opponentBoardIndex, targetStats, attackerStats);
    }

    public BoardBase GetBoard() {
        return board;
    }

    public BoardBase GetEnemyBoard() {
        return enemyBoard;
    }

    internal void RequestPlayCard(int handIndex, int boardIndex)
    {
        GameManager.Instance.CmdPlayUnitCard(handIndex, boardIndex);
        return;
    }

    internal void RequestAttack(int boardIndex, int opponentBoardIndex)
    {
        GameManager.Instance.CmdAttack(boardIndex, opponentBoardIndex);
        return;
    }

    internal void RequestCastSpell(int handIndex, Targeting targeting) {
        GameManager.Instance.CmdCastSpell(handIndex, targeting);
        return;
    }

    // Functions for running functions on server
    [Server]
    public void ServerAddCardToHand(UnitCardSO cardStatsSO) {
        UnitCardStats cardStats = new UnitCardStats(cardStatsSO);
        handController.AddCardToHand(cardStats);
    }

    [Server]
    public void ServerAddCardToOpponentHand() {
        enemyHand.AddCardToHand();
    }
}
