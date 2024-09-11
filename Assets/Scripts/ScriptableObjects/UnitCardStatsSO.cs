using UnityEngine;
using CardTypes;

[CreateAssetMenu(menuName = "Cards/UnitCard")]
public class UnitCardSO : BaseCardSO
{
    public int Attack;
    public int HP;

    private void OnEnable() {
        CardType = CardType.Unit;
    }
}
