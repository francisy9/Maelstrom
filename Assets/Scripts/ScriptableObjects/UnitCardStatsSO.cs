using UnityEngine;
using static Types;

[CreateAssetMenu(menuName = "Cards/UnitCard")]
public class UnitCardStatsSO : BaseCardSO
{
    public int attack;
    public int hp;

    private void OnEnable() {
        cardType = CardType.Unit;
    }
}
