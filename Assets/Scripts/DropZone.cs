using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("zone controller drop");
        if (eventData.pointerDrag) {
            Transform cardTransform = eventData.pointerDrag.GetComponent<RectTransform>();
            CardController card = cardTransform.gameObject.GetComponent<CardController>();
            Debug.Log($"parent has {transform.childCount} children before adding");
            cardTransform.SetParent(transform);
            card.PlayCard();
        }
    }
}
