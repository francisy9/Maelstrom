using UnityEngine;

namespace CardTypes
{
    public class BaseCard 
    {
        // Shared traits across all cards
        public CardType CardType { get; set; }
        public string CardName { get; set; }
        public string Description { get; set; }
        public int BaseManaCost { get; set; }
        public int CurrentManaCost { get; set; }
        // Majority of data stored on server
        // Go to card Effect for more details
        public CardEffect[] cardEffects { get; set; }

        public BaseCard() {}

        public CardType GetCardType() =>CardType;

        public int GetCurrentManaCost() => CurrentManaCost;

        public string GetCardName() => CardName;

        public static BaseCard Deserialize(byte[] serialized)
        {
            CardType cardType = (CardType)serialized[0];
            switch (cardType)
            {
                case CardType.Unit:
                    return UnitCardStats.DeserializeUnitCardByteArray(serialized);
                case CardType.Spell:
                    return SpellCardStats.DeserializeSpellCardByteArray(serialized);
                case CardType.Weapon:
                    return WeaponCardStats.DeserializeWeaponCardByteArray(serialized);
                default:
                    Debug.LogError("Type not implemented");
                    break;
            }
            return null;
        }
    }
}
