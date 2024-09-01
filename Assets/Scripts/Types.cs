using System;
using System.Collections.Generic;

public static class Types
{
    public enum CardType {
        Unit,
        Weapon,
        Spell,
    }

    public enum CardVisualEffect {
        None,
        Taunt,
        MultiStrike,
        Silenced,
        Enraged,
        PersistentEffect,
    }

    public class BaseCard 
    {
        // Shared traits across all cards
        public CardType CardType { get; private set; }
        public string CardName { get; private set; }
        public string Description { get; private set; }
        public int BaseManaCost { get; private set; }
        public int CurrentManaCost { get; set; }

        public BaseCard() {}
    }


    [Serializable]
    public class UnitCardStats
    {
        // Base stats that do not change
        public string CardName { get; private set; }
        public string Description { get; private set; }
        public int BaseManaCost { get; private set; }
        public int BaseAttack { get; private set; }
        public int BaseHP { get; private set; }

        // Mutable stats that can change during the game
        public int CurrentManaCost { get; set; }
        public int NumAttacks { get; set; }
        public int TotalNumAttacks { get; set; }
        public int CurrentAttack { get; set; }
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public List<CardVisualEffect> cardVisualEffects { get; set; }


        // Only stored on server
        public CardEffect[] cardEffects { get; set; }

        public UnitCardStats() {}

        // Constructor to initialize the base stats
        public UnitCardStats(UnitCardStatsSO cardStatsSO)
        {
            CardName = cardStatsSO.cardName;
            Description = cardStatsSO.description;
            BaseManaCost = cardStatsSO.manaCost;
            BaseAttack = cardStatsSO.attack;
            BaseHP = cardStatsSO.hp;

            // Initialize mutable stats with base stats
            CurrentManaCost = BaseManaCost;
            CurrentAttack = BaseAttack;
            CurrentHP = BaseHP;
            NumAttacks = 0;
            TotalNumAttacks = 1;
            MaxHP = BaseHP;
            cardVisualEffects = new List<CardVisualEffect>
            {
                CardVisualEffect.None
            };

            cardEffects = cardStatsSO.cardEffects;
        }
        
        // Note Serializing and deserializing only used to send number and text related stuff to client
        // So card specific logic is only stored on server
        public byte[] Serialize()
        {
            List<byte> serializedData = new List<byte>();

            byte[] cardNameBytes = System.Text.Encoding.UTF8.GetBytes(CardName);
            serializedData.Add((byte)cardNameBytes.Length);
            serializedData.AddRange(cardNameBytes);

            byte[] descriptionBytes = System.Text.Encoding.UTF8.GetBytes(Description);
            serializedData.Add((byte)descriptionBytes.Length);
            serializedData.AddRange(descriptionBytes);

            serializedData.Add((byte)BaseManaCost);
            serializedData.Add((byte)BaseAttack);
            serializedData.Add((byte)BaseHP);
            serializedData.Add((byte)CurrentManaCost);
            serializedData.Add((byte)NumAttacks);
            serializedData.Add((byte)TotalNumAttacks);
            serializedData.Add((byte)CurrentAttack);
            
            sbyte CurrentHp = (sbyte)CurrentHP;
            serializedData.Add((byte) CurrentHp); 

            serializedData.Add((byte)MaxHP);

            serializedData.Add((byte)cardVisualEffects.Count);
            foreach (var visualEffect in cardVisualEffects)
            {
                serializedData.Add((byte)visualEffect);
            }

            return serializedData.ToArray();
        }

        public static UnitCardStats Deserialize(byte[] serialized)
        {
            UnitCardStats cardStats = new UnitCardStats();

            int currentIndex = 0;

            int cardNameLength = serialized[currentIndex++];
            cardStats.CardName = System.Text.Encoding.UTF8.GetString(serialized, currentIndex, cardNameLength);
            currentIndex += cardNameLength;

            int descriptionLength = serialized[currentIndex++];
            cardStats.Description = System.Text.Encoding.UTF8.GetString(serialized, currentIndex, descriptionLength);
            currentIndex += descriptionLength;

            cardStats.BaseManaCost = serialized[currentIndex++];
            cardStats.BaseAttack = serialized[currentIndex++];
            cardStats.BaseHP = serialized[currentIndex++];
            cardStats.CurrentManaCost = serialized[currentIndex++];
            cardStats.NumAttacks = serialized[currentIndex++];
            cardStats.TotalNumAttacks = serialized[currentIndex++];
            cardStats.CurrentAttack = serialized[currentIndex++];

            byte currentHpByte = serialized[currentIndex++];
            sbyte currentHpSbyte = (sbyte) currentHpByte;
            cardStats.CurrentHP = currentHpSbyte;

            cardStats.MaxHP = serialized[currentIndex++];

            int visualEffectsLength = serialized[currentIndex++];
            cardStats.cardVisualEffects = new List<CardVisualEffect>();
            for (int i = 0; i < visualEffectsLength; i++)
            {
                cardStats.cardVisualEffects.Add((CardVisualEffect)serialized[currentIndex++]);
            }
            
            return cardStats;
        }
    }

    [Serializable]
    public class HeroStats
    {
        public int NumAttacks { get; set; }
        public int TotalNumAttacks { get; set; }
        public int CurrentAttack { get; set; }
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }

        public HeroStats() {}

        // Constructor
        public HeroStats(int hp) {
            NumAttacks = 0;
            TotalNumAttacks = 0;
            CurrentAttack = 0;
            MaxHP = hp;
            CurrentHP = hp;
        }

        public byte[] Serialize()
        {
            List<byte> serializedData = new List<byte>
            {
                (byte)NumAttacks,
                (byte)TotalNumAttacks,
                (byte)CurrentAttack
            };

            sbyte CurrentHp = (sbyte)CurrentHP;
            serializedData.Add((byte) CurrentHp); 
            
            serializedData.Add((byte)MaxHP);

            return serializedData.ToArray();
        }

        public static HeroStats Deserialize(byte[] serialized)
        {

            HeroStats heroStats = new HeroStats();

            int currentIndex = 0;

            heroStats.NumAttacks = serialized[currentIndex++];
            heroStats.TotalNumAttacks = serialized[currentIndex++];
            heroStats.CurrentAttack = serialized[currentIndex++];

            byte currentHpByte = serialized[currentIndex++];
            sbyte currentHpSbyte = (sbyte) currentHpByte;
            heroStats.CurrentHP = currentHpSbyte;

            heroStats.MaxHP = serialized[currentIndex++];
            
            return heroStats;
        }
    }

    public class WeaponStats
    {
        public string CardName { get; private set; }
        public string Description { get; private set; }
        public int Attack { get; set; }
        public int Durability { get; set; }

        public WeaponStats(WeaponStatsSO weaponStatsSO) {
            CardName = weaponStatsSO.CardName;
            Description = weaponStatsSO.Description;
            Attack = weaponStatsSO.Attack;
            Durability = weaponStatsSO.Durability;
        }
    }

    public static string PLAYER_CARD_LAYER = "PlayedCard";
    public static string OPPONENT_PLAYED_CARD_LAYER = "OpponentPlayedCard";
    public static string IN_HAND_CARD_LAYER = "InHandCard";
    public static string HERO_LAYER = "Hero";
    public static string OPPONENT_HERO_LAYER = "OpponentHero";
    public static int HERO_BOARD_INDEX = 7;
}
