using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class DragCardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InHandCardVisual cardVisual;
    private Canvas canvas;
    private Board playerDropZone;
    private Player player;
    public event EventHandler OnCardPlayed;
    private CanvasGroup canvasGroup;
    private Transform prevParentTransform;
    private HandController handController;
    private InHandCard card;
    private int uid;
    private int proposedBoardIndex;
    private Vector3 collapsedPos;
    private float collapsedRotation;
    private Vector3 expandedPos;
    private float expandedRotation;
    private bool canBeDragged;
    private CardType cardType;

    public void InitDragCardController(Player player, HandController handController, int uid) {
        canvas = player.GetCanvas();
        playerDropZone = player.GetDropZone();
        this.player = player;
        canvasGroup = GetComponent<CanvasGroup>();
        prevParentTransform = transform.parent;
        this.handController = handController;
        card = GetComponent<InHandCard>();
        this.uid = uid;
        canBeDragged = false;
        cardType = card.GetCardStats().CardType;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (HandVisual.Instance.IsExpanded()) {
            cardVisual.ProjectCardOnHover();
            canBeDragged = true;
        }
    }

    // TODO: Handle edge case where card was being hovered when another card was drawn
    public void OnPointerExit(PointerEventData eventData) {
        cardVisual.UnHoverCard();
        canBeDragged = false;
    }

    public void SetCollapsedPos(Vector3 pos, float angle) {
        collapsedPos = pos;
        collapsedRotation = angle;
    }

    public void SetExpandedPos(Vector3 pos, float angle) {
        expandedPos = pos;
        expandedRotation = angle;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canBeDragged) {
            eventData.pointerDrag = null;
            return;
        }

        if (!player.IsTurn()) {
            Debug.Log("Not your turn");
            eventData.pointerDrag = null;
            return;
        }

        if (player.GetMana() < card.GetCardStats().CurrentManaCost) {
            Debug.Log("Insufficient mana");
            eventData.pointerDrag = null;
            return;
        }

        cardVisual.BeingDrag();

        HandController.Instance.UnblockRayCasts();

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        handController.LocalTryingToPlayCard(uid);
        playerDropZone.UpdateXPos();
        transform.localEulerAngles = Vector3.zero;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);

        
        proposedBoardIndex = playerDropZone.GetProposedBoardIndex(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DropZoneIsPointerOver(eventData) && proposedBoardIndex != -1) {
            handController.RequestPlayCard(uid, proposedBoardIndex);
        } else {
            handController.ReturnCardToHand();
            playerDropZone.DestroyPlaceHolder();
            proposedBoardIndex = -1;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        HandController.Instance.BlockRayCasts();
    }

    private bool DropZoneIsPointerOver(PointerEventData eventData) {
        RectTransform dropZoneRectTransform = playerDropZone.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(dropZoneRectTransform, eventData.position, eventData.pressEventCamera);
    }

    public void ReturnCardToHand(int previousHandIndex) {
        transform.SetParent(prevParentTransform);
        transform.SetSiblingIndex(previousHandIndex);
        if (HandVisual.Instance.IsExpanded()) {
            transform.SetLocalPositionAndRotation(expandedPos, Quaternion.Euler(0, 0, expandedRotation));
        } else {
            transform.SetLocalPositionAndRotation(collapsedPos, Quaternion.Euler(0, 0, collapsedRotation));
        }
        cardVisual.EndDrag();
    }
}
