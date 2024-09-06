using UnityEngine;
using static Types;
[CreateAssetMenu(menuName = "Cards/SpellCard")]
public class SpellCardSO : BaseCardSO
{
    public int SpellDamage;
    private void OnEnable() {
        CardType = CardType.Spell;
    }
}
