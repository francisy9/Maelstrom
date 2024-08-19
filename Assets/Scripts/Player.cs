using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using static Types;

public class Player : NetworkBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Player opponent;
    [SerializeField] private Board board;
    [SerializeField] private EnemyBoard enemyBoard;
    [SerializeField] private Transform enemyHand;
    [SerializeField] private PlayerManaDisplay manaDisplay;
    [SerializeField] private PlayerManaDisplay enemyManaDisplay;
    [SerializeField] private List<CardStatsSO> deck;
    [SerializeField] private GameObject cardObject;
    [SerializeField] private HandController handController;
    [SerializeField] private GameObject cardBackObject;
    [SerializeField] private Button endTurnButton;
    [SyncVar (hook = nameof(OnManaChange))]
    private int mana = 0;
    [SyncVar (hook = nameof(OnManaChange))]
    private int maxMana = 0;
    [SyncVar]
    private int hp = 0;
    public event EventHandler OnStartTurn;
    [SyncVar] private bool inTurn;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        endTurnButton.onClick.AddListener(() => {
            RequestEndTurn();
        });
        handController.SetPlayer(this);
        board.SetPlayer(this);
    }

    [TargetRpc]
    public void TargetInitializeManaDisplay() {
        manaDisplay.InitializeManaDisplay(this);
        enemyManaDisplay.InitializeManaDisplay(opponent);
    }

    [TargetRpc]
    public void TargetStartGame(bool first) {
        // Debug.Log("Start game from Player.cs called");
    }

    public Board GetDropZone() {
        return board;
    }

    [TargetRpc]
    public void TargetStartTurn() {
        // Debug.Log($"{name} starting turn");
        inTurn = true;
        OnStartTurn?.Invoke(this, EventArgs.Empty);
    }

    private void OnManaChange(int _, int _a) {
        manaDisplay.UpdateManaVisual();
        enemyManaDisplay.UpdateManaVisual();
    }

    [TargetRpc]
    public void TargetStartOpponentTurn() {
        // Debug.Log("Opponent Turn");
    }

    public void IncrementMaxMana() {
        if (isServer)
        {
            maxMana += 1;
        }
    }

    [Server]
    public void ConsumeMana(int mana) {
        this.mana -= mana;
    }

    [Server]
    public void RefreshMana() {
        mana = maxMana;
    }

    [TargetRpc]
    public void AddCardToHand(CardStatsSO cardStatsSO) {
        CardStats cardStats = new CardStats(cardStatsSO);
        handController.AddCardToHand(cardStats);
    }

    [TargetRpc]
    public void AddCardToOpponentHand() {
        Instantiate(cardBackObject, enemyHand);
    }

    public Canvas GetCanvas() {
        return canvas;
    }

    public int GetMana() {
        return mana;
    }

    public int GetTotalMana() {
        return maxMana;
    }

    public int GetHp() {
        return hp;
    }

    public bool IsTurn() {
        return inTurn;
    }

    private void ResetCardAttacks() {
        board.ResetCardAttacks();
    }

    private void RequestEndTurn()
    {
        if (GameManager.Instance == null) {
            Debug.LogError("Game manager instance is null");
        }
        GameManager.Instance.CmdEndTurn();
    }

    [TargetRpc]
    public void TargetEndTurn() {
        // Debug.Log("handling end turn response");
        ResetCardAttacks();
        inTurn = false;
    }

    [TargetRpc]
    public void TargetPlayCard(int boardIndex) {
        handController.PlayCard(boardIndex);
    }

    [TargetRpc]
    public void TargetOpponentPlayCard(byte[] cardData, int boardIndex) {
        CardStats cardStats = CardStats.Deserialize(cardData);
        enemyBoard.PlaceCardOnBoard(cardStats, boardIndex);
        return;
    }

    [TargetRpc]
    public void TargetAttackCard(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) {
        CardStats cardStats = CardStats.Deserialize(cardData);
        CardStats opponentCardStats = CardStats.Deserialize(opponentCardData);

        board.UpdateCardAfterAttack(boardIndex, cardStats);
        enemyBoard.UpdateCardAfterAttack(opponentBoardIndex, opponentCardStats);
    }

    internal void RequestPlayCard(int handIndex, int boardIndex)
    {
        GameManager.Instance.CmdPlayCard(handIndex, boardIndex);
        return;
    }

    internal void RequestAttack(int boardIndex, int opponentBoardIndex)
    {
        GameManager.Instance.CmdAttack(boardIndex, opponentBoardIndex);
        return;
    }
}
