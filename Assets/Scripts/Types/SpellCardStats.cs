using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CardTypes
{
    [Serializable]
    public class SpellCardStats : BaseCard
    {
        // Base stats that do not change
        public int BaseSpellDamage { get; set; }

        // Mutable stats that can change during the game
        public int CurrentSpellDamage { get; set; }

        public SpellCardStats()
        {
            CardType = CardType.Spell;
        }

        // Constructor to initialize the base stats
        public SpellCardStats(SpellCardSO spellCardSO)
        {
            CardName = spellCardSO.CardName;
            CardType = CardType.Spell;
            Description = spellCardSO.Description;
            BaseManaCost = spellCardSO.ManaCost;
            BaseSpellDamage = spellCardSO.SpellDamage;

            // Initialize mutable stats with base stats
            CurrentManaCost = BaseManaCost;
            CurrentSpellDamage = BaseSpellDamage;

            cardEffects = spellCardSO.CardEffects;
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
            serializedData.Add((byte)BaseSpellDamage);
            serializedData.Add((byte)CurrentManaCost);
            serializedData.Add((byte)CurrentSpellDamage);

            serializedData.Add((byte)cardEffects.Length);
            foreach (var cardEffect in cardEffects)
            {
                byte[] effectData = cardEffect.Serialize();
                serializedData.AddRange(effectData);
            }
            return serializedData.ToArray();
        }

        public static SpellCardStats DeserializeSpellCardByteArray(byte[] serialized)
        {
            SpellCardStats cardStats = new SpellCardStats();

            // Starting at 1 since first byte is card type
            int currentIndex = 1;

            int cardNameLength = serialized[currentIndex++];
            cardStats.CardName = System.Text.Encoding.UTF8.GetString(serialized, currentIndex, cardNameLength);
            currentIndex += cardNameLength;

            int descriptionLength = serialized[currentIndex++];
            cardStats.Description = System.Text.Encoding.UTF8.GetString(serialized, currentIndex, descriptionLength);
            currentIndex += descriptionLength;

            cardStats.BaseManaCost = serialized[currentIndex++];
            cardStats.BaseSpellDamage = serialized[currentIndex++];
            cardStats.CurrentManaCost = serialized[currentIndex++];
            cardStats.CurrentSpellDamage = serialized[currentIndex++];

            int cardEffectsLength = serialized[currentIndex++];
            List<CardEffect> cardEffectsList = new List<CardEffect>();
            for (int i = 0; i < cardEffectsLength; i++)
            {
                int remainingBytes = serialized.Length - currentIndex;
                if (remainingBytes < CardEffect.SerializedNumBytes())
                {
                    Debug.LogError($"Insufficient data for card effect {i + 1}/{cardEffectsLength}. Stopping effect deserialization.");
                    break;
                }
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
