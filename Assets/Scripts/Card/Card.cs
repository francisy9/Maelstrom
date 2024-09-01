using System;
using UnityEngine;
using static Types;

public class Card : MonoBehaviour
{
    [SerializeField] private CardVisual cardVisual;
    private UnitCardStats cardStats;

    public void InitCard(UnitCardStats cardStats) {
        this.cardStats = cardStats;
        cardVisual.InitVisual();
    }

    public void DestroySelf() {
        transform.SetParent(null);
        Destroy(gameObject);
    }

    public UnitCardStats GetCardStats() {
        return cardStats;
    }
}
