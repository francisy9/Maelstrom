using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectTargetType {
    Self,
    Single,
    Multiple,
    Board,
    EnemyBoard,
    WholeBoard,
}

public enum TriggerTime {
    None,
    OnPlay,
    OnDeath,
    OnAttack,
    OnDamaged,
}

public enum EffectType {
    Buff, // e.g. +1 hp
    Damage, // e.g. deal 3 damage to target
    Change, // e.g. set target attack to 5
    Convert, // e.g. change target unit to another unit
    GiveEffect, // e.g. give a unit a on death effect/wind fury etc.
    Heal, // e.g. heal unit for 5 hp
    Spawn, // e.g. spawn an ogre on board
    Taunt,
    MultiStrike,
}

public enum TargetSelection {
    None,
    Random,
    Adjacent,
    UserSelect,
}

public enum Targetable {
    None,
    AllAllies,
    AllEnemies,
    AllyUnits,
    EnemyUnits,
    All,
}

public enum BuffEffect {
    None,
    MultiStrike,
    Taunt,
}

[System.Serializable]
public class CardEffect
{
    public EffectTargetType effectTargetType;
    public EffectType effectType;
    public TriggerTime triggerTime;
    public TargetSelection targetSelection;
    public Targetable targetable;
    public int damageVal;
    public int healVal;
    public int attackBuffVal;
    public int hpBuffVal;
    public BuffEffect buffEffect;
    public bool needsTargeting;
    public int numTargets;
    public UnitCardStatsSO spawnUnitCardStatsSO;
}
