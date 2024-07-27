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
            DragCardController card = cardTransform.gameObject.GetComponent<DragCardController>();
            CardLocation currentCardLocation = card.GetCardLocation();
            if (currentCardLocation == CardLocation.Hand) {
                cardTransform.SetParent(transform);
                card.PlayCard(transform);
            }
        }
    }
}
