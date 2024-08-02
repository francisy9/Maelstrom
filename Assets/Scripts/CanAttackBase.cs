using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CanAttackBase : MonoBehaviour
{
    public int remainingAttacks;
    public int totalAttacks;
    public int attackVal;
    public int remainingHp;
    public event EventHandler OnAttack;
    public event EventHandler OnBeingAttacked;
    public event EventHandler OnDeath;

    public int GetRemainingHp() {
        return remainingHp;
    }

    public int GetAttackVal() {
        return attackVal;
    }

    public bool CanAttack() {
        if (attackVal <= 0) {
            Debug.Log("Unit's attack value is 0!");
            return false;
        }

        if (remainingAttacks <= 0) {
            Debug.Log("Wait until next turn to attack!");
            return false;
        }

        return true;
    }

    public void Attack(CanAttackBase target) {
        remainingHp -= target.GetAttackVal();
        remainingAttacks -= 1;
        target.AttackedBy(this);
        OnAttack?.Invoke(this, EventArgs.Empty);
        if (remainingHp <= 0) {
            Death();
        }
    }

    public void AttackedBy(CanAttackBase attacker) {
        remainingHp -= attacker.GetAttackVal();
        OnBeingAttacked?.Invoke(this, EventArgs.Empty);
    }

    public void ResetAttack() {
        remainingAttacks = totalAttacks;
    }

    public abstract void Death();
}
