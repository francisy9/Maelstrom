using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CardVisual cardVisual;
    public event EventHandler OnCardPlayed;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public static Transform zone;
    
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("On begin drag");
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void PlayCard() {
        OnCardPlayed.Invoke(this, EventArgs.Empty);
        // cardVisual.PlayCard();
    }
}
