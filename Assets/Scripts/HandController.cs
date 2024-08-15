using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private static HandController Instance;
    private List<Card> cards;
    [SerializeField] private GameObject cardObject;
    private Player player;
    [SerializeField] private Board board;

    private void Awake() {
        Instance = this;
        cards = new List<Card>();
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void AddCardToHand(CardStatsSO cardStatsSO) {
        GameObject currentCardObject = Instantiate(cardObject, transform);
        Card cardComponent = currentCardObject.GetComponent<Card>();
        DragCardController dragCardControllerComponent = currentCardObject.GetComponent<DragCardController>();
        cardComponent.InitCard(cardStatsSO);
        dragCardControllerComponent.InitDragCardController(player, cards.Count);
        cards.Add(cardComponent);
    }

    public void PlayCard(int i) {
        Card card = cards[i];
        CardStatsSO cardStatsSO = card.GetCardStatsSO();
        DestroyCardAtIndex(i);
        board.PlaceCardOnBoard(cardStatsSO);
    }

    private void DestroyCardAtIndex(int i) {
        cards[i].DestroySelf();
        cards.RemoveAt(i);
    }

    public CardStatsSO GetCardStatsSOOnIndex(int i) {
        return cards[i].GetCardStatsSO();
    }
}
