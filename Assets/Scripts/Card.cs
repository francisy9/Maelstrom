using System;
using UnityEngine;

public class Card : CanAttackBase
{
    [SerializeField] private CardStatsSO cardStatsSO;
    [SerializeField] private CardVisual cardVisual;
    private DragCardController dragCardController;
    private int manaCost;

    public void InitCard(CardStatsSO cardStatsSO) {
        this.cardStatsSO = cardStatsSO;
        dragCardController = GetComponent<DragCardController>();
        dragCardController.OnCardPlayed += DragCardController_OnCardPlayed;
        cardVisual.InitVisual();
        manaCost = cardStatsSO.manaCost;
    }
 
    public void DragCardController_OnCardPlayed(object sender, EventArgs e)
    {
        inPlayStats.attacks = 0;
        inPlayStats.totalAttacks = 1;
        inPlayStats.attackVal = cardStatsSO.attack;
        inPlayStats.hp = cardStatsSO.hp;
    }

    public CardStatsSO GetCardStatsSO() {
        return cardStatsSO;
    }

    public int GetManaCost() {
        Debug.Log($"{name} has {manaCost} mana");
        return manaCost;
    }

    public override void Death() {
        // OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
