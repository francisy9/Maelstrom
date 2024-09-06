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
    // Sent to client
    public EffectTargetType effectTargetType;
    public EffectType effectType;
    public TriggerTime triggerTime;
    // Sent to client
    public TargetSelection targetSelection;
    // Sent to client
    public Targetable targetable;
    public int damageVal;
    public int healVal;
    public int attackBuffVal;
    public int hpBuffVal;
    public BuffEffect buffEffect;
    // Sent to client
    public bool needsTargeting;
    // Sent to client
    public int numTargets;
    public UnitCardSO spawnUnitCardStatsSO;

    public byte[] Serialize() {
        List<byte> serializedData = new List<byte>
        {
            (byte)effectTargetType,
            (byte)targetSelection,
            (byte)targetable,
            (byte)(needsTargeting ? 1 : 0),
            (byte)numTargets
        };

        return serializedData.ToArray();
    }

    public static CardEffect Deserialize(byte[] serialized) {
        CardEffect cardEffect = new CardEffect();

            int currentIndex = 0;

            cardEffect.effectTargetType = (EffectTargetType)serialized[currentIndex++];
            cardEffect.targetSelection = (TargetSelection)serialized[currentIndex++];
            cardEffect.targetable = (Targetable)serialized[currentIndex++];
            cardEffect.needsTargeting = serialized[currentIndex++] != 0;
            cardEffect.numTargets = serialized[currentIndex++];
            
            return cardEffect;
    }

    public static int SerializedNumBytes() {
        return 5;
    }
}
