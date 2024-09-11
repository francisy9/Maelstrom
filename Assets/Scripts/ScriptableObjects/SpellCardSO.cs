using UnityEngine;
using CardTypes;

[CreateAssetMenu(menuName = "Cards/SpellCard")]
public class SpellCardSO : BaseCardSO
{
    public int SpellDamage;
    private void OnEnable() {
        CardType = CardType.Spell;
    }
}
