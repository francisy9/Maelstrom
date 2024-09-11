using UnityEngine;
using CardTypes;

[CreateAssetMenu(menuName = "Cards/WeaponCard")]
public class WeaponSO : BaseCardSO
{
    public int Attack;
    public int Durability;
    private void OnEnable() {
        CardType = CardType.Weapon;
    }
}
