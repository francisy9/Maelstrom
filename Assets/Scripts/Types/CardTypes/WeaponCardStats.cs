using System;
using System.Collections;
using System.Collections.Generic;

namespace CardTypes
{
    [Serializable]
    public class WeaponCardStats : BaseCard
    {
        // Base stats that do not change
        public int BaseAttack { get; set; }
        public int BaseDurability { get; set; }

        // Mutable stats that can change during the game
        public int CurrentDurability { get; set; }
        public int CurrentAttack { get; set; }
        public List<CardVisualEffect> cardVisualEffects { get; set; }

        public WeaponCardStats()
        {
            CardType = CardType.Weapon;
        }

        // Constructor to initialize the base stats
        public WeaponCardStats(WeaponSO cardStatsSO)
        {
            CardName = cardStatsSO.CardName;
            CardType = CardType.Weapon;
            Description = cardStatsSO.Description;
            BaseManaCost = cardStatsSO.ManaCost;
            BaseAttack = cardStatsSO.Attack;
            BaseDurability = cardStatsSO.Durability;

            // Initialize mutable stats with base stats
            CurrentManaCost = BaseManaCost;
            CurrentAttack = BaseAttack;
            CurrentDurability = BaseDurability;
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
            throw new NotImplementedException();
        }

        public static WeaponCardStats DeserializeWeaponCardByteArray(byte[] serialized)
        {
            WeaponCardStats cardStats = new WeaponCardStats();

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
            cardStats.BaseDurability = serialized[currentIndex++];
            cardStats.CurrentManaCost = serialized[currentIndex++];
            cardStats.CurrentAttack = serialized[currentIndex++];
            cardStats.CurrentDurability = serialized[currentIndex++];

            return cardStats;
        }
    }
}
