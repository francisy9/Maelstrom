using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class Board : MonoBehaviour
{
    private Player player;
    private List<OnBoardCard> onBoardCards;
    private List<float> cardXPos;
    [SerializeField] private GameObject onBoardCardObject;
    [SerializeField] private GameObject cardPlaceHolderObject;
    private GameObject cardPlaceHolder;
    private int proposedIndex;

    private void Awake() {
        onBoardCards = new List<OnBoardCard>();
        cardXPos = new List<float>();
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void ResetCardAttacks() {
        foreach (Transform child in transform) {
            OnBoardCard card = child.GetComponent<OnBoardCard>();
            card.ResetAttack();
        }
            
    }

    public void PlaceCardOnBoard(CardStatsSO cardStatsSO, InPlayStats inPlayStats, int boardIndex) {
        GameObject cardObject = Instantiate(onBoardCardObject, transform);
        cardObject.transform.SetSiblingIndex(boardIndex);
        OnBoardCard cardComponent = cardObject.GetComponent<OnBoardCard>();
        OnBoardDragController dragCardControllerComponent = cardObject.GetComponent<OnBoardDragController>();
        cardComponent.InitCard(cardStatsSO, inPlayStats);
        dragCardControllerComponent.InitDragCardController(player);
        onBoardCards.Insert(boardIndex, cardComponent);
        DestroyPlaceHolder();
    }

    public void UpdateXPos() {
        List<float> xPos = new List<float>();
        foreach (Transform child in transform) {
            xPos.Add(child.localPosition.x);
        }
        cardXPos = xPos;
    }

    public int GetProposedBoardIndex(PointerEventData eventData) {
        if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, eventData.position, eventData.pressEventCamera)) {
            DestroyPlaceHolder();
            Debug.Log("Place holder destroyed since mouse not over drop zone");
            return -1;
        };

        Vector2 mouseOverPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out mouseOverPos);
        Debug.Log(mouseOverPos.x);
        int index = 0;
        for (int i = 0; i < cardXPos.Count; i++) {
            if (mouseOverPos.x > cardXPos[i]) {
                index = i + 1;
            } else {
                break;
            }
        }
        if (cardPlaceHolder == null) {
            CreatePlaceHolder(index);
            proposedIndex = index;
        } else if (proposedIndex != index) {
            cardPlaceHolder.transform.SetSiblingIndex(index);
            proposedIndex = index;
        }
        return index;
    }

    private GameObject CreatePlaceHolder(int index) {
        GameObject placeHolder = Instantiate(cardPlaceHolderObject, transform);
        placeHolder.transform.SetSiblingIndex(index);
        cardPlaceHolder = placeHolder;
        return placeHolder;
    }

    public void DestroyPlaceHolder() {
        if (cardPlaceHolder) {
            Destroy(cardPlaceHolder);
        }
        cardPlaceHolder = null;
    }
}
