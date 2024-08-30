using System;
using UnityEngine;
using static Types;

public class Card : MonoBehaviour
{
    [SerializeField] private CardVisual cardVisual;
    private CardStats cardStats;

    public void InitCard(CardStats cardStats) {
        this.cardStats = cardStats;
        cardVisual.InitVisual();
    }

    public void DestroySelf() {
        transform.SetParent(null);
        Destroy(gameObject);
    }

    public CardStats GetCardStats() {
        return cardStats;
    }
}
