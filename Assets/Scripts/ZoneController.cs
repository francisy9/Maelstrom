using UnityEngine;
using UnityEngine.EventSystems;

public class ZoneController : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("zone controller drop");
        if (eventData.pointerDrag) {
            Debug.Log($"parent has {transform.childCount} children before adding");
            eventData.pointerDrag.GetComponent<RectTransform>().SetParent(transform);
        }
    }
}
