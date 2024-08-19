using System;
using UnityEngine;
using static Types;

public class Card : CanAttackBase
{
    [SerializeField] private CardVisual cardVisual;

    public void InitCard(CardStats cardStats) {
        this.cardStats = cardStats;
        cardVisual.InitVisual();
    }

    public int GetManaCost() {
        return cardStats.CurrentManaCost;
    }

    public override void Death() {
        // OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
