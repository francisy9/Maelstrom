using System;

namespace CardTypes
{
    public enum CardType
    {
        Unit,
        Weapon,
        Spell,
    }

    public enum CardVisualEffect
    {
        None,
        Taunt,
        MultiStrike,
        Silenced,
        Enraged,
        PersistentEffect,
    }

    public enum TargetType {
        Ally,
        Enemy,
    }

    public class Targeting {
        public int targetBoardIndex;
        public TargetType targetType;
    }
}