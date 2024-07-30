using System;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardStatsSO cardStatsSO;
    [SerializeField] private CardVisual cardVisual;
    private DragCardController dragCardController;
    private int remainingAttacks;
    private int totalAttacks;
    private int attackVal;
    private int remainingHp;
    private int manaCost;
    public event EventHandler OnAttack;
    public event EventHandler OnBeingAttacked;
    public event EventHandler OnDeath;

    public void InitCard(CardStatsSO cardStatsSO) {
        this.cardStatsSO = cardStatsSO;
        dragCardController = GetComponent<DragCardController>();
        dragCardController.OnCardPlayed += DragCardController_OnCardPlayed;
        cardVisual.InitVisual();
        manaCost = cardStatsSO.manaCost;
    }
 
    private void DragCardController_OnCardPlayed(object sender, EventArgs e)
    {
        remainingAttacks = 0;
        totalAttacks = 1;
        attackVal = cardStatsSO.attack;
        remainingHp = cardStatsSO.hp;
    }

    public CardStatsSO GetCardStatsSO() {
        return cardStatsSO;
    }

    public int GetManaCost() {
        Debug.Log($"{name} has {manaCost} mana");
        return manaCost;
    }

    public int GetRemainingHp() {
        return remainingHp;
    }

    public int GetAttackVal() {
        return attackVal;
    }

    public void AttackCard(Card opponentCard) {
        remainingHp -= opponentCard.GetAttackVal();
        remainingAttacks = 0;
        opponentCard.AttackedBy(this);
        OnAttack?.Invoke(this, EventArgs.Empty);
        if (remainingHp <= 0) {
            Death();
        }
    }

    public void AttackedBy(Card opponentCard) {
        remainingHp -= opponentCard.GetAttackVal();
        OnBeingAttacked?.Invoke(this, EventArgs.Empty);
    }

    private void Death() {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }
}
