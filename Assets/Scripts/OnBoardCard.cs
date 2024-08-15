using System;
using UnityEngine;

public class OnBoardCard : CanAttackBase
{
    private CardStatsSO cardStatsSO;
    [SerializeField] private OnBoardCardVisual onBoardCardVisual;
    private OnBoardDragController dragCardController;

    public void InitCard(CardStatsSO cardStatsSO) {
        this.cardStatsSO = cardStatsSO;
        dragCardController = GetComponent<OnBoardDragController>();
        onBoardCardVisual.InitVisual();
        inPlayStats.attacks = 0;
        inPlayStats.totalAttacks = 1;
        inPlayStats.attackVal = cardStatsSO.attack;
        inPlayStats.hp = cardStatsSO.hp;
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
