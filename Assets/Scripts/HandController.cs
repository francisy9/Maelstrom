using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Types;

public class HandController : MonoBehaviour
{
    private static HandController Instance;
    private Dictionary<int, Card> cardHashMap;
    [SerializeField] private GameObject cardObject;
    private Player player;
    [SerializeField] private Board board;
    private int cardUid = 0;
    private int currentlyDraggingCardHandIndex;
    private int tryingToPlayCardUID;
    // Used to prevent race condition
    private bool handlingAction;
    

    private void Awake() {
        Instance = this;
        cardHashMap = new Dictionary<int, Card>();
        handlingAction = false;
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void AddCardToHand(CardStatsSO cardStatsSO) {
        GameObject currentCardObject = Instantiate(cardObject, transform);
        Card cardComponent = currentCardObject.GetComponent<Card>();
        DragCardController dragCardControllerComponent = currentCardObject.GetComponent<DragCardController>();
        cardComponent.InitCard(cardStatsSO);
        dragCardControllerComponent.InitDragCardController(player, this, cardUid);
        cardHashMap.Add(cardUid, cardComponent);
        cardUid += 1;
    }

    public void PlayCard(InPlayStats inPlayStats, int boardIndex) {
        Card card = cardHashMap[tryingToPlayCardUID];
        CardStatsSO cardStatsSO = card.GetCardStatsSO();
        cardHashMap.Remove(tryingToPlayCardUID);
        tryingToPlayCardUID = -1;
        currentlyDraggingCardHandIndex = -1;
        board.PlaceCardOnBoard(cardStatsSO, inPlayStats, boardIndex);
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

        Card card = cardHashMap[cardUid];
        // TODO: Make it so that DragCardController passed card statso so here !
        player.RequestPlayCard(currentlyDraggingCardHandIndex, card.GetCardStatsSO(), boardIndex);
    }
}
