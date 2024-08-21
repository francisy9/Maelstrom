using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Types;

public class HandController : MonoBehaviour
{
    public static HandController Instance;
    private Dictionary<int, Card> cardHashMap;
    [SerializeField] private GameObject cardObject;
    private Player player;
    [SerializeField] private Board board;
    private int cardUid = 0;
    private int currentlyDraggingCardHandIndex;
    private int tryingToPlayCardUID;
    // Used to prevent race condition
    private bool handlingAction;
    public event EventHandler CardAddedToHand;
    

    private void Awake() {
        Instance = this;
        cardHashMap = new Dictionary<int, Card>();
        handlingAction = false;
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void AddCardToHand(CardStats cardStats) {
        GameObject currentCardObject = Instantiate(cardObject, transform);
        Card cardComponent = currentCardObject.GetComponent<Card>();
        DragCardController dragCardControllerComponent = currentCardObject.GetComponent<DragCardController>();
        cardComponent.InitCard(cardStats);
        dragCardControllerComponent.InitDragCardController(player, this, cardUid);
        cardHashMap.Add(cardUid, cardComponent);
        cardUid += 1;
        CardAddedToHand.Invoke(this, EventArgs.Empty);
    }

    public void PlayCard(int boardIndex) {
        Card card = cardHashMap[tryingToPlayCardUID];
        CardStats cardStats = card.GetCardStats();
        cardHashMap.Remove(tryingToPlayCardUID);
        tryingToPlayCardUID = -1;
        currentlyDraggingCardHandIndex = -1;
        board.PlaceCardOnBoard(cardStats, boardIndex);
        card.DestroySelf();
        handlingAction = false;
    }

    public void ReturnCardToHand() {
        DragCardController dragCardController = cardHashMap[tryingToPlayCardUID].GetComponent<DragCardController>();
        dragCardController.ReturnCardToHand(currentlyDraggingCardHandIndex);
        handlingAction = false;
    }

    // Called when card begins to be dragged
    public void LocalTryingToPlayCard(int cardId) {
        Card card = cardHashMap[cardId];
        currentlyDraggingCardHandIndex = card.transform.GetSiblingIndex();
        tryingToPlayCardUID = cardId;
        handlingAction = true;
    }

    public bool IsHandControllerBusy() {
        return handlingAction;
    }

    // Make call to server to play card
    public void RequestPlayCard(int cardUid, int boardIndex) {
        if (cardUid != tryingToPlayCardUID) {
            Debug.LogError("Previously dragging card and currently attempting to play card seem to be different");
        }

        player.RequestPlayCard(currentlyDraggingCardHandIndex, boardIndex);
    }

    public int GetNumCards() {
        return cardHashMap.Count;
    }
}
