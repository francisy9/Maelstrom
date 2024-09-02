using System;
using System.Collections.Generic;
using UnityEngine;
using static Types;

public class HandController : MonoBehaviour
{
    public static HandController Instance;
    private Dictionary<int, InHandCard> cardHashMap;
    [SerializeField] private GameObject cardObject;
    private Player player;
    [SerializeField] private Board board;
    private int cardUid = 0;
    private int currentlyDraggingCardHandIndex;
    private int tryingToPlayCardUID;
    // Used to prevent race condition
    public event EventHandler CardAddedToHand;
    public event EventHandler OnCardPlayed;
    

    private void Awake() {
        Instance = this;
        cardHashMap = new Dictionary<int, InHandCard>();
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void AddCardToHand(BaseCard cardStats) {
        GameObject currentCardObject = Instantiate(cardObject, transform);
        InHandCard cardComponent = currentCardObject.GetComponent<InHandCard>();
        DragCardController dragCardControllerComponent = currentCardObject.GetComponent<DragCardController>();
        cardComponent.InitCard(cardStats);
        dragCardControllerComponent.InitDragCardController(player, this, cardUid);
        cardHashMap.Add(cardUid, cardComponent);
        currentCardObject.layer = LayerMask.NameToLayer(IN_HAND_CARD_LAYER);
        cardUid += 1;
        CardAddedToHand.Invoke(this, EventArgs.Empty);
    }

    public void PlayUnitCard(int boardIndex) {
        InHandCard card = cardHashMap[tryingToPlayCardUID];
        BaseCard cardStats = card.GetCardStats();
        cardHashMap.Remove(tryingToPlayCardUID);
        tryingToPlayCardUID = -1;
        currentlyDraggingCardHandIndex = -1;
        board.PlaceCardOnBoard(cardStats as UnitCardStats, boardIndex, player);
        card.DestroySelf();
        OnCardPlayed.Invoke(this, EventArgs.Empty);
    }

    public void ReturnCardToHand() {
        DragCardController dragCardController = cardHashMap[tryingToPlayCardUID].GetComponent<DragCardController>();
        dragCardController.ReturnCardToHand(currentlyDraggingCardHandIndex);
    }

    // Called when card begins to be dragged
    public void LocalTryingToPlayCard(int cardId) {
        InHandCard card = cardHashMap[cardId];
        currentlyDraggingCardHandIndex = card.transform.GetSiblingIndex();
        tryingToPlayCardUID = cardId;
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

    public void BlockRayCasts() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
    }

    public void UnblockRayCasts() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
    }
}
