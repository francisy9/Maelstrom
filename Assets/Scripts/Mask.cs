using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mask : MonoBehaviour, IPointerClickHandler
{
    public event EventHandler HandTapped;

    private void Awake()
    {
        EnableMask();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandTapped?.Invoke(this, EventArgs.Empty);
        DisableMask();
    }

    public void EnableMask()
    {
        gameObject.SetActive(true);
    }

    private void DisableMask()
    {
        gameObject.SetActive(false);
    }
}