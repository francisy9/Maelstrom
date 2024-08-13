using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Player opponent;
    [SerializeField] private int hp;
    public int remainingMana = 0;
    private int maxMana = 0;
    [SerializeField] private Board board;
    [SerializeField] private Transform hand;
    [SerializeField] private Board enemyBoard;
    [SerializeField] private Transform enemyHand;
    [SerializeField] private PlayerManaDisplay manaDisplay;
    [SerializeField] private PlayerManaDisplay enemyManaDisplay;
    [SerializeField] private List<CardStatsSO> deck;
    [SerializeField] private GameObject cardObject;
    [SerializeField] private GameObject cardBackObject;
    public event EventHandler ManaConsumed;
    public event EventHandler OnStartTurn;
    public event EventHandler OnGameStart;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
    }

    [ClientRpc]
    public void InitializeManaDisplay() {
        if (isLocalPlayer) {
            manaDisplay.InitializeManaDisplay(this);
            enemyManaDisplay.InitializeManaDisplay(opponent);
        }
    }

    [ClientRpc]
    public void StartGame(bool first) {
        Debug.Log("Start game from Player.cs called");
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    public Board GetDropZone() {
        return board;
    }

    public void SetMaxMana(int mana) {
        maxMana = mana;
    }

    [ClientRpc]
    public void StartTurn() {
        maxMana += 1;
        remainingMana = maxMana;
        UpdateMana();
        enemyManaDisplay.UpdateManaVisual(this, EventArgs.Empty);
    }

    public void UpdateMana() {
        manaDisplay.UpdateManaVisual(this, EventArgs.Empty);
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

    public int GetRemainingMana() {
        return remainingMana;
    }

    public int GetTotalMana() {
        return maxMana;
    }

    public void ConsumeMana(int manaCost) {
        remainingMana -= manaCost;
        ManaConsumed?.Invoke(this, EventArgs.Empty);
    }

    public bool IsTurn() {
        bool isHostsTurn = GameManager.Instance.IsP1Turn();
        if (isServer) {
            return isHostsTurn;
        } else {
            return !isHostsTurn;
        }
    }

    public void ResetCardAttacks() {
        board.ResetCardAttacks();
    }
}
