using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroBase : CanAttackBase
{
    private int totalHp;
    private int hp;
    private int attackVal;
    private int remainingAttacks;
    private int numAttacks;
    public event EventHandler OnAttack;
    public event EventHandler OnBeingAttacked;
    [SerializeField] HeroVisual heroVisual;

    public virtual void InitHero(int hp) {
        totalHp = hp;
        this.hp = totalHp;
        heroVisual.InitVisual();
    }

    public int GetTotalHp() {
        return totalHp;
    }

    public override int GetRemainingHp() {
        return hp;
    }

    public override int GetAttackVal() {
        return attackVal;
    }

    public override int GetRemainingNumAttacks() {
        return remainingAttacks;
    }

    public override void ResetAttack() {
        remainingAttacks = numAttacks;
    }
}
