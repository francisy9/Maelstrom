using System;
using System.Collections.Generic;
using UnityEngine;
using CardTypes;
using static Layers.Layers;

public class HandController : MonoBehaviour
{
    public static HandController Instance;
    private List<DragCardController> cardControllerList;
    [SerializeField] private GameObject cardObject;
    private Player player;
    [SerializeField] private Board board;
    public event EventHandler CardAddedToHand;
    public event EventHandler OnCardPlayed;
    private bool isHandlingCard = false;

    private void Awake() {
        Instance = this;
        cardControllerList = new List<DragCardController>();
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void AddCardToHand(BaseCard cardStats) {
        GameObject currentCardObject = Instantiate(cardObject, transform);
        InHandCard cardComponent = currentCardObject.GetComponent<InHandCard>();
        DragCardController dragCardControllerComponent = currentCardObject.GetComponent<DragCardController>();
        cardComponent.InitCard(cardStats);
        dragCardControllerComponent.InitDragCardController(player, this);
        cardControllerList.Add(dragCardControllerComponent);
        currentCardObject.layer = LayerMask.NameToLayer(IN_HAND_CARD_LAYER);
        CardAddedToHand.Invoke(this, EventArgs.Empty);
    }

    public void PlayUnitCard(int handIndex, int boardIndex) {
        DragCardController dragCardController = cardControllerList[handIndex];
        InHandCard card = dragCardController.GetCard();
        cardControllerList.RemoveAt(handIndex);
        board.PlaceCardOnBoard(card.GetCardStats() as UnitCardStats, boardIndex, player);
        card.DestroySelf();
        OnCardPlayed?.Invoke(this, EventArgs.Empty);
        isHandlingCard = false;
    }

    public void PlaySpellCard(int handIndex) {
        DragCardController dragCardController = cardControllerList[handIndex];
        InHandCard card = dragCardController.GetCard();
        cardControllerList.RemoveAt(handIndex);
        card.DestroySelf();
        OnCardPlayed?.Invoke(this, EventArgs.Empty);
        isHandlingCard = false;
    }

    public int FindCardIndex(DragCardController dragCardController) {
        for (int i = 0; i < cardControllerList.Count; i++) {
            if (cardControllerList[i] == dragCardController) {
                return i;
            }
        }
        Debug.LogError("Card not found");
        return -1;
    }

    // Make call to server to play card
    public void RequestPlayUnitCard(DragCardController dragCardController, int boardIndex) {
        if (isHandlingCard) {
            dragCardController.ReturnCardToHand();
            return;
        }
        int handIndex = FindCardIndex(dragCardController);
        player.RequestPlayUnitCard(handIndex, boardIndex);
        isHandlingCard = true;
    }

    public void RequestCastSpell(DragCardController dragCardController, Targeting targeting) {
        if (isHandlingCard) {
            dragCardController.ReturnCardToHand();
            return;
        }
        int handIndex = FindCardIndex(dragCardController);
        player.RequestCastSpell(handIndex, targeting);
        isHandlingCard = true;
    }

    public int GetNumCards() {
        return cardControllerList.Count;
    }

    public void BlockRayCasts() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
    }

    public void UnblockRayCasts() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
    }

    public bool IsHandlingCard() {
        return isHandlingCard;
    }
}
