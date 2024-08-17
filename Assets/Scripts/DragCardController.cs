using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class DragCardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private Board playerDropZone;
    private Player player;
    public event EventHandler OnCardPlayed;
    private CanvasGroup canvasGroup;
    private Transform prevParentTransform;
    private HandController handController;
    private Card card;
    private int uid;
    private int proposedBoardIndex;

    public void InitDragCardController(Player player, HandController handController, int uid) {
        canvas = player.GetCanvas();
        playerDropZone = player.GetDropZone();
        this.player = player;
        canvasGroup = GetComponent<CanvasGroup>();
        prevParentTransform = transform.parent;
        this.handController = handController;
        this.card = GetComponent<Card>();
        this.uid = uid;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!player.IsTurn()) {
            Debug.Log("Not your turn");
            eventData.pointerDrag = null;
            return;
        }

        if (player.GetMana() < card.GetCardStatsSO().manaCost) {
            Debug.Log("Insufficient mana");
            eventData.pointerDrag = null;
            return;
        }

        if (handController.IsHandControllerBusy()) {
            Debug.Log("Hand Controller busy");
            eventData.pointerDrag = null;
            return;
        }

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        handController.LocalTryingToPlayCard(uid);
        playerDropZone.UpdateXPos();
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);

        proposedBoardIndex = playerDropZone.GetProposedBoardIndex(eventData);
        Debug.Log(proposedBoardIndex);
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
    }

    private bool DropZoneIsPointerOver(PointerEventData eventData) {
        RectTransform dropZoneRectTransform = playerDropZone.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(dropZoneRectTransform, eventData.position, eventData.pressEventCamera);
    }

    public void ReturnCardToHand(int previousHandIndex) {
        transform.SetParent(prevParentTransform);
        transform.SetSiblingIndex(previousHandIndex);
    }
}
