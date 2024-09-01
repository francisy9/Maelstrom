using UnityEngine;

[CreateAssetMenu()]
public class UnitCardStatsSO : ScriptableObject
{
    public string cardName;
    public string description;
    public int manaCost;
    public int attack;
    public int hp;

    public bool EqualValues(object other)
    {
        if (other == null || GetType() != other.GetType())
            return false;

        UnitCardStatsSO otherCard = (UnitCardStatsSO)other;

        return cardName == otherCard.cardName &&
               description == otherCard.description &&
               manaCost == otherCard.manaCost &&
               attack == otherCard.attack &&
               hp == otherCard.hp;
    }
}
