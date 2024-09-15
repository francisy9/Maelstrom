using System.Collections.Generic;
using Mirror;
using UnityEngine;
using CardTypes;
using ResponseTypes;

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
    [SerializeField] private PlayerAnimationManager animationManager;

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
    public void TargetBeginGame(bool first) => communicationManager.TargetBeginGame(first);
    public void TargetInitializeHeroes(int heroStartHp, int enemyHeroStartHp) => communicationManager.TargetInitializeHeroes(heroStartHp, enemyHeroStartHp);
    public void TargetAddCardToHand(byte[] serializedCardArray) => communicationManager.TargetAddCardToHand(serializedCardArray);
    public void TargetAddCardToOpponentHand() => communicationManager.TargetAddCardToOpponentHand();
    public void TargetPlayUnitCard(int handIndex, int boardIndex) => communicationManager.TargetPlayUnitCard(handIndex, boardIndex);
    public void TargetOpponentUnitPlayCard(byte[] cardData, int handIndex, int boardIndex) => communicationManager.TargetOpponentPlayUnitCard(cardData, handIndex, boardIndex);
    public void TargetPlaySpellCard(int handIndex) => communicationManager.TargetPlaySpellCard(handIndex);
    public void TargetOpponentPlaySpellCard(int handIndex) => communicationManager.TargetOpponentPlaySpellCard(handIndex);
    public void TargetAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) => communicationManager.TargetAttackResponse(boardIndex, opponentBoardIndex, cardData, opponentCardData);
    public void TargetOpponentAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) => communicationManager.TargetOpponentAttackResponse(boardIndex, opponentBoardIndex, cardData, opponentCardData);
    public void TargetExecuteCardEffect(byte[] responseData) => communicationManager.TargetExecuteCardEffect(responseData);
    public void TargetExecuteOpponentCardEffect(byte[] responseData) => communicationManager.TargetExecuteOpponentCardEffect(responseData);
    internal void RequestPlayUnitCard(int handIndex, int boardIndex) => communicationManager.RequestPlayUnitCard(handIndex, boardIndex);
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
    public PlayerAnimationManager GetAnimationManager() => animationManager;
}
