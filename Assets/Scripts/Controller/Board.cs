using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class Board : BoardBase
{
    public static Board Instance;
    private Player player;
    private List<float> cardXPos;
    [SerializeField] private GameObject cardPlaceHolderObject;
    private GameObject cardPlaceHolder;
    private int proposedIndex;

    public override void Awake() {
        base.Awake();
        cardXPos = new List<float>();
        Instance = this;
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

    public override void PlaceCardOnBoard(UnitCardStats cardStats, int boardIndex, Player player) {
        base.PlaceCardOnBoard(cardStats, boardIndex, player);
        cardPlaceHolder.transform.SetParent(null); // Need this since Destroy isn't immediate
        DestroyPlaceHolder();
        onBoardCards[boardIndex].gameObject.layer = LayerMask.NameToLayer(PLAYER_CARD_LAYER);
        UpdateBoardIndexHashMap();
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
            return -1;
        };

        Vector2 mouseOverPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out mouseOverPos);

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
