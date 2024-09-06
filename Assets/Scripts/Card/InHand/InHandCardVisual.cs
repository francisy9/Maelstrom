using UnityEngine;

public class InHandCardVisual : InHandCardVisualBase
{
    [SerializeField] private GameObject hoverCardVisualObject;
    private GameObject hoverCardObject;
    private bool enlarged;
    private bool beingDragged;

    public virtual void InitVisual()
    {
        base.InitVisual();
        cardType = cardStats.CardType;
        enlarged = false;
        beingDragged = false;
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    public void ProjectCardOnHover() {
        if (enlarged || beingDragged) {
            return;
        }
        canvasGroup.alpha = 0f;
        hoverCardObject = Instantiate(hoverCardVisualObject, HandController.Instance.transform);
        InHandCardVisualBase hoverCard = hoverCardObject.GetComponentInChildren<InHandCardVisualBase>();
        hoverCard.InitVisual(cardStats);
        hoverCardObject.transform.localScale *= 1.2f;
        Vector3 hoverPos = transform.position;
        hoverPos.y += 100;
        hoverCardObject.transform.position = hoverPos;
        enlarged = true;
    }

    public void UnHoverCard() {
        if (enlarged) {
            Destroy(hoverCardObject);
            hoverCardObject.transform.SetParent(null);
            canvasGroup.alpha = 1f;
            enlarged = false;
        }
    }

    public void BeingDrag() {
        UnHoverCard();
        beingDragged = true;
    }

    public void EndDrag() {
        beingDragged = false;
    }
 }
