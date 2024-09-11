using System.Collections.Generic;

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
    Buff, // e.g. +1 hp or give multi strike
    Damage, // e.g. deal 3 damage to target
    Change, // e.g. set target attack to 5
    Convert, // e.g. change target unit to another unit
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

// Sent to client
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
    // Sent to client
    public bool needsTargeting;
    // Sent to client
    public int numTargets;
    public UnitCardSO spawnUnitCardStatsSO;

    // If changing this, also change Deserialize and SerializedNumBytes
    public byte[] Serialize() {
        List<byte> serializedData = new List<byte>
        {
            (byte)targetable,
            (byte)(needsTargeting ? 1 : 0),
            (byte)numTargets
        };

        return serializedData.ToArray();
    }

    // If changing this, also change Serialize and SerializedNumBytes
    public static CardEffect Deserialize(byte[] serialized) {
        CardEffect cardEffect = new CardEffect();

        int currentIndex = 0;

        cardEffect.targetable = (Targetable)serialized[currentIndex++];
        cardEffect.needsTargeting = serialized[currentIndex++] != 0;
        cardEffect.numTargets = serialized[currentIndex++];
        
        return cardEffect;
    }

    public static int SerializedNumBytes() {
        return 3;
    }
}
