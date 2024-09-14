using System;
using System.Collections.Generic;
using System.Linq;

namespace CardTypes
{
    [Serializable]
    public class UnitCardStats : BaseCard
    {
        // Base stats that do not change
        public int BaseAttack { get; set; }
        public int BaseHP { get; set; }

        // Mutable stats that can change during the game
        public int NumAttacks { get; set; }
        public int TotalNumAttacks { get; set; }
        public int CurrentAttack { get; set; }
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public List<CardVisualEffect> cardVisualEffects { get; set; }

        public UnitCardStats()
        {
            CardType = CardType.Unit;
        }

        // Constructor to initialize the base stats
        public UnitCardStats(UnitCardSO cardStatsSO)
        {
            CardName = cardStatsSO.CardName;
            CardType = CardType.Unit;
            Description = cardStatsSO.Description;
            BaseManaCost = cardStatsSO.ManaCost;
            BaseAttack = cardStatsSO.Attack;
            BaseHP = cardStatsSO.HP;

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

            cardEffects = cardStatsSO.CardEffects;
        }

        // Note Serializing and deserializing only used to send number and text related stuff to client
        // So card specific logic is only stored on server
        public byte[] Serialize()
        {
            List<byte> serializedData = new List<byte>
            {
                (byte)CardType
            };

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
            serializedData.Add((byte)CurrentHp);

            serializedData.Add((byte)MaxHP);

            serializedData.Add((byte)cardVisualEffects.Count);
            foreach (var visualEffect in cardVisualEffects)
            {
                serializedData.Add((byte)visualEffect);
            }

            serializedData.Add((byte)cardEffects.Length);
            foreach (var cardEffect in cardEffects)
            {
                serializedData.Concat(cardEffect.Serialize());
            }
            return serializedData.ToArray();
        }

        public static UnitCardStats DeserializeUnitCardByteArray(byte[] serialized)
        {
            UnitCardStats cardStats = new UnitCardStats();

            // Starting at 1 since first byte is card type
            int currentIndex = 1;

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
            sbyte currentHpSbyte = (sbyte)currentHpByte;
            cardStats.CurrentHP = currentHpSbyte;

            cardStats.MaxHP = serialized[currentIndex++];

            int visualEffectsLength = serialized[currentIndex++];
            cardStats.cardVisualEffects = new List<CardVisualEffect>();
            for (int i = 0; i < visualEffectsLength; i++)
            {
                cardStats.cardVisualEffects.Add((CardVisualEffect)serialized[currentIndex++]);
            }

            int cardEffectsLength = serialized[currentIndex++];
            List<CardEffect> cardEffectsList = new List<CardEffect>();
            for (int i = 0; i < cardEffectsLength; i++)
            {
                byte[] effectData = new byte[CardEffect.SerializedNumBytes()];
                Array.Copy(serialized, currentIndex, effectData, 0, CardEffect.SerializedNumBytes());
                cardEffectsList.Add(CardEffect.Deserialize(effectData));

                currentIndex += CardEffect.SerializedNumBytes();
            }

            cardStats.cardEffects = cardEffectsList.ToArray();

            return cardStats;
        }
    }
}