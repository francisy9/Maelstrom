using System;
using UnityEngine;

public abstract class CanAttackBase : MonoBehaviour
{
    public event EventHandler OnDeath;

    public abstract int GetRemainingHp();

    public abstract int GetAttackVal();

    public abstract int GetRemainingNumAttacks();

    public bool CanAttack() {
        if (GetAttackVal() <= 0) {
            Debug.Log("Unit's attack value is 0!");
            return false;
        }

        if (GetRemainingNumAttacks() <= 0) {
            Debug.Log("No attacks remaining");
            return false;
        }

        return true;
    }

    public abstract void ResetAttack();

    public abstract void Death();
}
