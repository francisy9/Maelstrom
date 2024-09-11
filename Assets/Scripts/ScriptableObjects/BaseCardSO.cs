using UnityEngine;
using CardTypes;

[System.Serializable]

public class BaseCardSO : ScriptableObject
{
    public string CardName;
    public CardType CardType;
    public string Description;
    public int ManaCost;
    public CardEffect[] CardEffects;

    public CardType GetCardType() {
        return CardType;
    }
}
