using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandVisual : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Mask mask;
    private HandController handController;
    private float collapsedWidth;
    [SerializeField] private float expandedWidth;
    private int numCards;
    private bool expanded;
    private RectTransform rectTransform;
    private float fanAngle;
    [SerializeField] private float collapsedFanAngle;
    [SerializeField] private float expandedFanAngle;
    [SerializeField] private float collapsedWidthOffset;
    [SerializeField] private float expandedWidthXOffset;
    [SerializeField] private float collapsedYOffset;
    [SerializeField] private float expandedYOffset;
    private float angleStep;

    private void Awake() {
        expanded = false;
        rectTransform = transform as RectTransform;
        collapsedWidth = rectTransform.rect.width;
    }
    
    private void Start() {
        handController = GetComponent<HandController>();
        handController.CardAddedToHand += HandController_CardAddedToHand;
        mask.HandTapped += Mask_HandTapped;
    }

    private void Mask_HandTapped(object sender, EventArgs e)
    {
        ExpandHandArea();
    }

    private void HandController_CardAddedToHand(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        numCards = handController.GetNumCards();
        int i  = 0;
        fanAngle = expanded ? expandedFanAngle : collapsedFanAngle;

        if (numCards > 1) {
            angleStep = fanAngle * 2 / (numCards - 1);
        } else {
            angleStep = fanAngle;
            i = 1;
        }

        foreach(Transform child in transform) {
            ArrangeCard(i, child);
            i++;
        }
        return;
    }

    private void ArrangeCard(int cardIndex, Transform cardTransform) {
        float width = expanded ? expandedWidth : (collapsedWidth - collapsedWidthOffset);
        float angle = -fanAngle + angleStep * cardIndex;
        float rad = angle * Mathf.Deg2Rad;
        float yOffset = expanded ? expandedYOffset : collapsedYOffset;

        float x = width * Mathf.Sin(rad);
        float y = width * Mathf.Cos(rad) - yOffset;

        if (expanded) {
            x = (width - expandedWidthXOffset) * Mathf.Sin(rad);
        }

        cardTransform.SetLocalPositionAndRotation(new UnityEngine.Vector3(x , y, 0), UnityEngine.Quaternion.Euler(0, 0, -angle));
        return;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (expanded && !IsPointerOverHandArea()) {
                CollapseHandArea();
            }
        }

        UpdateVisual();
    }

    private bool IsPointerOverHandArea() {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, canvas.worldCamera);
    }

    public void ExpandHandArea() {
        expanded = true;
        UnityEngine.Vector2 sizeDelta = rectTransform.sizeDelta;
        UnityEngine.Vector3 pos = rectTransform.localPosition;

        sizeDelta.x = expandedWidth;
        rectTransform.sizeDelta = sizeDelta;

        pos.x -= (canvas.GetComponent<RectTransform>().rect.width - collapsedWidth) / 2;
        rectTransform.localPosition = pos;
    }

    public void CollapseHandArea() {
        expanded = false;
        UnityEngine.Vector2 sizeDelta = rectTransform.sizeDelta;
        UnityEngine.Vector3 pos = rectTransform.localPosition;

        sizeDelta.x = collapsedWidth;
        rectTransform.sizeDelta = sizeDelta;

        pos.x += (canvas.GetComponent<RectTransform>().rect.width - collapsedWidth) / 2;
        rectTransform.localPosition = pos;
        mask.EnableMask();
    }
}
