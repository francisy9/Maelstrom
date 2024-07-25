using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class DragCardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CardVisual cardVisual;
    public event EventHandler OnCardPlayed;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public static Transform zone;
    private CardLocation cardLocation;
    private Transform prevParentTransform;
    
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start() {
        prevParentTransform = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("On begin drag");
        switch (cardLocation) {
            case CardLocation.Hand:
                canvasGroup.alpha = .6f;
                canvasGroup.blocksRaycasts = false;
                transform.SetParent(transform.root);
                break;
            case CardLocation.Board:
                break;
            case CardLocation.Discard:
                // Debug.LogError("Should not be able to drag discarded card");
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (cardLocation) {
            case CardLocation.Hand:
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
                break;
            case CardLocation.Board:
                break;
            case CardLocation.Discard:
                Debug.LogError("Should not be able to drag discarded card");
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(prevParentTransform);
    }

    public void PlayCard(Transform boardTransform) {
        cardLocation = CardLocation.Board;
        prevParentTransform = boardTransform;
        OnCardPlayed.Invoke(this, EventArgs.Empty);
    }

    public CardLocation GetCardLocation() {
        return cardLocation;
    }
}
