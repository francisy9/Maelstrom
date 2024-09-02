using UnityEngine;
using static Types;

[System.Serializable]

public class BaseCardSO : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public string description;
    public int manaCost;
    public CardEffect[] cardEffects;
}
