using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Types;

public abstract class CanAttackBase : MonoBehaviour
{
    public InPlayStats inPlayStats;
    public event EventHandler OnAttack;
    public event EventHandler OnBeingAttacked;
    public event EventHandler OnDeath;

    private void Awake() {
        inPlayStats = new InPlayStats();
    }

    public int GetRemainingHp() {
        return inPlayStats.hp;
    }

    public int GetAttackVal() {
        return inPlayStats.attackVal;
    }

    public bool CanAttack() {
        if (inPlayStats.attackVal <= 0) {
            Debug.Log("Unit's attack value is 0!");
            return false;
        }

        if (inPlayStats.attacks <= 0) {
            Debug.Log("Wait until next turn to attack!");
            return false;
        }

        return true;
    }

    public void Attack(CanAttackBase target) {
        inPlayStats.hp -= target.GetAttackVal();
        inPlayStats.attacks -= 1;
        target.AttackedBy(this);
        OnAttack?.Invoke(this, EventArgs.Empty);
        if (inPlayStats.hp <= 0) {
            Death();
        }
    }

    public void AttackedBy(CanAttackBase attacker) {
        inPlayStats.hp -= attacker.GetAttackVal();
        OnBeingAttacked?.Invoke(this, EventArgs.Empty);
    }

    public void ResetAttack() {
        inPlayStats.attacks = inPlayStats.totalAttacks;
    }

    public abstract void Death();
}
