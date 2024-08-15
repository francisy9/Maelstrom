using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class Board : MonoBehaviour
{
    private Player player;
    private List<OnBoardCard> onBoardCards;
    [SerializeField] private GameObject onBoardCardObject;

    private void Awake() {
        onBoardCards = new List<OnBoardCard>();
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void ResetCardAttacks() {
        foreach (Transform child in transform) {
            Card card = child.GetComponent<Card>();
            card.ResetAttack();
        }
            
    }

    public void PlaceCardOnBoard(CardStatsSO cardStatsSO) {
        GameObject cardObject = Instantiate(onBoardCardObject, transform);
        OnBoardCard cardComponent = cardObject.GetComponent<OnBoardCard>();
        OnBoardDragController dragCardControllerComponent = cardObject.GetComponent<OnBoardDragController>();
        cardComponent.InitCard(cardStatsSO);
        dragCardControllerComponent.InitDragCardController(player);
        onBoardCards.Add(cardComponent);
    }
}
