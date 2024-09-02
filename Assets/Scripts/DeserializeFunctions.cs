using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Types;

public class DeserializeFunctions
{
    public static UnitCardStats DeserializeUniCardByteArray(byte[] serialized)
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
            sbyte currentHpSbyte = (sbyte) currentHpByte;
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
