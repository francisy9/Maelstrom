using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private int hp;
    private int remainingMana;
    private int maxMana;
    [SerializeField] private DropZone dropZone;
    [SerializeField] private bool isServer;
    [SerializeField] private Transform hand;
    [SerializeField] private List<CardStatsSO> deck;
    [SerializeField] private GameObject cardObject;

    public DropZone GetDropZone() {
        return dropZone;
    }

    public bool IsServer() {
        return isServer;
    }

    public void SetMaxMana(int mana) {
        maxMana = mana;
    }

    public void IncrementMaxMana() {
        maxMana += 1;
    }

    public void StartTurn() {
        remainingMana = maxMana;
    }

    public void DrawCard() {
        if (deck.Count > 0)
        {
            int cardIndex = Random.Range(0, deck.Count);
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

    public void ConsumeMana(int manaCost) {
        remainingMana -= manaCost;
    }

    public bool IsTurn() {
        bool isHostsTurn = GameManager.Instance.IsHostsTurn();
        if (isServer) {
            return isHostsTurn;
        } else {
            return !isHostsTurn;
        }
    }
}
