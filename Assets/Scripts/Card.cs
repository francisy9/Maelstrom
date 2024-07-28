using System;
using UnityEngine;
using static Types;

public class Card : MonoBehaviour
{
    [SerializeField] private CardStatsSO cardStatsSO;
    private DragCardController dragCardController;
    public CardStats cardStats;
    private int remainingAttacks;
    private int attackVal;
    private int remainingHp;
    public event EventHandler OnAttack;
    public event EventHandler OnBeingAttacked;

    private void Awake() {
        cardStats = new CardStats
        {
            Hp = cardStatsSO.hp,
            Attack = cardStatsSO.attack,
            Mana = cardStatsSO.manaCost,
            CardState = CardState.CardFront
        };
    }

    private void Start() {
        dragCardController = GetComponent<DragCardController>();
        dragCardController.OnCardPlayed += DragCardController_OnCardPlayed;
    }

    private void DragCardController_OnCardPlayed(object sender, EventArgs e)
    {
        remainingAttacks = 1;
        attackVal = cardStats.Attack;
        remainingHp = cardStats.Hp;
    }

    public CardStatsSO GetCardStatsSO() {
        return cardStatsSO;
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
        Debug.Log($"{this.name} has hp val {remainingHp} and is dead"); // Implement death logic
    }
}
