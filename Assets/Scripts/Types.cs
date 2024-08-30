using System;
using System.Collections.Generic;

public static class Types
{
    public enum CardState {
        CardBack,
        CardFront,
        OnBoard,
    }


    [System.Serializable]
    public class CardStats
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

        public CardStats() {}

        // Constructor to initialize the base stats
        public CardStats(CardStatsSO cardStatsSO)
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
        }

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

            return serializedData.ToArray();
        }

        public static CardStats Deserialize(byte[] serialized)
        {
            CardStats cardStats = new CardStats();

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
            
            return cardStats;
        }
    }

    public static string PLAYER_CARD_LAYER = "PlayedCard";
    public static string OPPONENT_PLAYED_CARD_LAYER = "OpponentPlayedCard";
    public static string IN_HAND_CARD_LAYER = "InHandCard";
    public static string HERO_LAYER = "Hero";
    public static string OPPONENT_HERO_LAYER = "OpponentHero";
}
