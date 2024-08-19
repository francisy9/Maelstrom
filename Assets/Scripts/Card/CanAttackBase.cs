using System;
using UnityEngine;
using static Types;

public abstract class CanAttackBase : MonoBehaviour
{
    public CardStats cardStats;
    public event EventHandler OnAttack;
    public event EventHandler OnBeingAttacked;
    public event EventHandler OnDeath;

    public int GetRemainingHp() {
        return cardStats.CurrentHP;
    }

    public int GetAttackVal() {
        return cardStats.CurrentAttack;
    }

    public bool CanAttack() {
        if (cardStats.CurrentAttack <= 0) {
            Debug.Log("Unit's attack value is 0!");
            return false;
        }

        if (cardStats.NumAttacks <= 0) {
            Debug.Log("No attacks remaining");
            return false;
        }

        return true;
    }

    public void Attack(CanAttackBase target) {
        cardStats.CurrentHP -= target.GetAttackVal();
        cardStats.NumAttacks -= 1;
        target.AttackedBy(this);
        OnAttack?.Invoke(this, EventArgs.Empty);
        if (cardStats.CurrentHP <= 0) {
            Death();
        }
    }

    public void AttackedBy(CanAttackBase attacker) {
        cardStats.CurrentHP -= attacker.GetAttackVal();
        OnBeingAttacked?.Invoke(this, EventArgs.Empty);
    }

    public void ResetAttack() {
        cardStats.NumAttacks = cardStats.TotalNumAttacks;
    }

    public CardStats GetCardStats() {
        return cardStats;
    }

    public abstract void Death();
}
