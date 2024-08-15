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
    private Card card;
    private int handIndex;

    public void InitDragCardController(Player player, int handIndex) {
        canvas = player.GetCanvas();
        playerDropZone = player.GetDropZone();
        this.player = player;
        canvasGroup = GetComponent<CanvasGroup>();
        prevParentTransform = transform.parent;
        card = GetComponent<Card>();
        this.handIndex = handIndex;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!player.IsTurn()) {
            Debug.Log("Not your turn");
            eventData.pointerDrag = null;
            return;
        }

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform);

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DropZoneIsPointerOver(eventData)) {
            player.RequestPlayCard(handIndex);
        } else {
            transform.SetParent(prevParentTransform);
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private bool DropZoneIsPointerOver(PointerEventData eventData) {
        RectTransform dropZoneRectTransform = playerDropZone.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(dropZoneRectTransform, eventData.position, eventData.pressEventCamera);
    }
}
