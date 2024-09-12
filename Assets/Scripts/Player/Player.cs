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
    [SerializeField] private List<UnitCardSO> deck;
    [SerializeField] private PlayerManaSystem manaManager;
    [SerializeField] private PlayerTurnManager turnManager;
    [SerializeField] private PlayerHandManager handManager;
    [SerializeField] private PlayerBoardManager boardManager;
    [SerializeField] private PlayerCommunicationManager communicationManager;
    [SerializeField] private PlayerHeroManager heroManager;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        InitializeComponents();
    }

    private void InitializeComponents() {
        manaManager.Initialize(this);
        turnManager.Initialize(this);
        handManager.Initialize(this);
        communicationManager.Initialize(this);
        boardManager.Initialize(this);
        heroManager.Initialize(this);
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


    // Request and Responses
    public void TargetBeginGame(bool first, int heroStartHp, int enemyHeroStartHp) => communicationManager.TargetBeginGame(first, heroStartHp, enemyHeroStartHp);
    public void TargetAddCardToHand(byte[] serializedCardArray) => communicationManager.TargetAddCardToHand(serializedCardArray);
    public void TargetAddCardToOpponentHand() => communicationManager.TargetAddCardToOpponentHand();
    public void TargetPlayCard(int handIndex, int boardIndex) => communicationManager.TargetPlayCard(handIndex, boardIndex);
    public void TargetOpponentPlayCard(byte[] cardData, int handIndex, int boardIndex) => communicationManager.TargetOpponentPlayCard(cardData, handIndex, boardIndex);
    internal void RequestPlayCard(int handIndex, int boardIndex) => communicationManager.RequestPlayCard(handIndex, boardIndex);
    public void TargetAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) => communicationManager.TargetAttackResponse(boardIndex, opponentBoardIndex, cardData, opponentCardData);
    public void TargetOpponentAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) => communicationManager.TargetOpponentAttackResponse(boardIndex, opponentBoardIndex, cardData, opponentCardData);
    internal void RequestAttack(int boardIndex, int opponentBoardIndex) => communicationManager.RequestAttack(boardIndex, opponentBoardIndex);
    internal void RequestCastSpell(int handIndex, Targeting targeting) => communicationManager.RequestCastSpell(handIndex, targeting);

    public Canvas GetCanvas() => canvas;
    public Hero GetHero() => heroManager.GetHero();
    public EnemyHero GetEnemyHero() => heroManager.GetEnemyHero();
    public Player GetOpponent() => opponent;
    public PlayerHandManager GetHandManager() => handManager;
    public PlayerBoardManager GetBoardManager() => boardManager;
    public PlayerHeroManager GetHeroManager() => heroManager;
    public Board GetBoard() => boardManager.GetBoard();
    public EnemyBoard GetEnemyBoard() => boardManager.GetEnemyBoard();
}
