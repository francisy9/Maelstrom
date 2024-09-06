using UnityEngine;
using static Types;

[CreateAssetMenu(menuName = "Cards/WeaponCard")]
public class WeaponSO : BaseCardSO
{
    public int Attack;
    public int Durability;
    private void OnEnable() {
        CardType = CardType.Weapon;
    }
}
