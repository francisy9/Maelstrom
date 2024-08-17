using System;
using UnityEngine;
using static Types;

public class OnBoardCard : CanAttackBase
{
    private CardStatsSO cardStatsSO;
    [SerializeField] private OnBoardCardVisual onBoardCardVisual;

    public void InitCard(CardStatsSO cardStatsSO, InPlayStats inPlayStats) {
        this.cardStatsSO = cardStatsSO;
        onBoardCardVisual.InitVisual();
        this.inPlayStats = inPlayStats;
    }

    public CardStatsSO GetCardStatsSO() {
        return cardStatsSO;
    }

    public override void Death() {
        // OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
