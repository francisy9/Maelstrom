using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Player opponent;
    [SerializeField] private Board board;
    [SerializeField] private Transform hand;
    [SerializeField] private Board enemyBoard;
    [SerializeField] private Transform enemyHand;
    [SerializeField] private PlayerManaDisplay manaDisplay;
    [SerializeField] private PlayerManaDisplay enemyManaDisplay;
    [SerializeField] private List<CardStatsSO> deck;
    [SerializeField] private GameObject cardObject;
    [SerializeField] private GameObject cardBackObject;
    [SerializeField] private Button endTurnButton;
    [SyncVar (hook = nameof(OnManaChange))]
    private int mana = 0;
    [SyncVar (hook = nameof(OnManaChange))]
    private int maxMana = 0;
    [SyncVar] private int health = 0;
    public event EventHandler OnStartTurn;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        endTurnButton.onClick.AddListener(() => {
            RequestEndTurn();
        });
    }

    [TargetRpc]
    public void TargetInitializeManaDisplay() {
        manaDisplay.InitializeManaDisplay(this);
        enemyManaDisplay.InitializeManaDisplay(opponent);
    }

    [TargetRpc]
    public void TargetStartGame(bool first) {
        Debug.Log("Start game from Player.cs called");
    }

    public Board GetDropZone() {
        return board;
    }

    [TargetRpc]
    public void TargetStartTurn() {
        Debug.Log($"{name} starting turn");
        OnStartTurn?.Invoke(this, EventArgs.Empty);
    }

    private void OnManaChange(int _, int _a) {
        manaDisplay.UpdateManaVisual();
        enemyManaDisplay.UpdateManaVisual();
    }

    [TargetRpc]
    public void TargetStartOpponentTurn() {
        Debug.Log("Opponent Turn");
    }

    public void IncrementMaxMana() {
        if (isServer)
        {
            maxMana += 1;
        }
    }

    public void ConsumeMana(int mana) {
        if (isServer)
        {
            this.mana -= mana;
        }
    }

    public void RefreshMana() {
        if (isServer) {
            mana = maxMana;
        }
    }

    public void DrawCards(int numCardsToDraw)
    {
        List<CardStatsSO> drawnCardStatsSOs = DrawCardsFromDeck(numCardsToDraw);
        RpcUpdateHand(drawnCardStatsSOs);
    }

    public List<CardStatsSO> DrawCardsFromDeck(int numCardsToDraw) {
        List<CardStatsSO> cardStatsSOs = new List<CardStatsSO>();
        for (int i = 0; i < numCardsToDraw; i++) {
            if (deck.Count > 0)
            {
                int cardIndex = UnityEngine.Random.Range(0, deck.Count);
                cardStatsSOs.Add(deck[cardIndex]);
            } else {
                cardStatsSOs.Add(null);
            }
        }
        return cardStatsSOs;
    }

    [ClientRpc]
    void RpcUpdateHand(List<CardStatsSO> cardStatsSOs)
    {
        if (isLocalPlayer)
        {
            AddCardsToHand(cardStatsSOs);
         } else {
            // Update the opponent's view with a card back
            AddCardToOpponentHand(cardStatsSOs.Count);
        }
    }


    // TODO: deal with overdraw
    private void AddCardsToHand(List<CardStatsSO> cardStatsSOs) {
        for (int i = 0; i < cardStatsSOs.Count; i++) {
            GameObject currentCardObject = Instantiate(cardObject, hand);
            Card cardComponent = currentCardObject.GetComponent<Card>();
            DragCardController dragCardControllerComponent = currentCardObject.GetComponent<DragCardController>();
            cardComponent.InitCard(cardStatsSOs[i]);
            dragCardControllerComponent.InitDragCardController(this);
        }
    }

    private void AddCardToOpponentHand(int numCards) {
        for (int i = 0; i < numCards; i++) {
            Instantiate(cardBackObject, enemyHand);
        }
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

    public bool IsTurn() {
        bool isHostsTurn = GameManager.Instance.IsP1Turn();
        if (isServer) {
            return isHostsTurn;
        } else {
            return !isHostsTurn;
        }
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
        Debug.Log("handling end turn response");
        ResetCardAttacks();
    }
}
