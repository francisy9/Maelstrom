using UnityEngine;

[CreateAssetMenu(menuName = "Cards/UnitCard")]
public class UnitCardStatsSO : ScriptableObject
{
    public string cardName;
    public string description;
    public int manaCost;
    public int attack;
    public int hp;
    public CardEffect[] cardEffects;
}
