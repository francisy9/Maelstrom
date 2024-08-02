using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private int hp;
    public int remainingMana;
    private int maxMana;
    [SerializeField] private Board board;
    [SerializeField] private bool isServer;
    [SerializeField] private Transform hand;
    [SerializeField] private List<CardStatsSO> deck;
    [SerializeField] private GameObject cardObject;
    public event EventHandler ManaConsumed;
    public event EventHandler OnStartTurn;
    public event EventHandler OnGameStart;

    public void StartGame(bool first) {
        maxMana = 0;
        remainingMana = 0;
        OnGameStart?.Invoke(this, EventArgs.Empty);
        int numCardsToDraw = 3;
        if (!first) {
            numCardsToDraw = 4;
        }
        for(int i = 0; i < numCardsToDraw; i++) {
            DrawCard();
        }
    }

    public Board GetDropZone() {
        return board;
    }

    public bool IsServer() {
        return isServer;
    }

    public void SetMaxMana(int mana) {
        maxMana = mana;
    }

    public void StartTurn() {
        maxMana += 1;
        remainingMana = maxMana;
        OnStartTurn?.Invoke(this, EventArgs.Empty);
    }

    public void DrawCard() {
        if (deck.Count > 0)
        {
            int cardIndex = UnityEngine.Random.Range(0, deck.Count);
            CardStatsSO cardStatsSO = deck[cardIndex];
            GameObject currentCardObject = Instantiate(cardObject, hand);
            Card cardComponent = currentCardObject.GetComponent<Card>();
            DragCardController dragCardControllerComponent = currentCardObject.GetComponent<DragCardController>();
            cardComponent.InitCard(cardStatsSO);
            dragCardControllerComponent.InitDragCardController(this);
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
        bool isHostsTurn = GameManager.Instance.IsHostsTurn();
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
