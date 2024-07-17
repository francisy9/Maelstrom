using UnityEngine;
using static Types;

public class Card : MonoBehaviour
{
    [SerializeField] private CardStatsSO cardStatsSO;
    public CardStats cardStats;
    
    private void Awake() {
        cardStats = new CardStats
        {
            Hp = cardStatsSO.hp,
            Attack = cardStatsSO.attack,
            Mana = cardStatsSO.manaCost,
            CardState = CardState.CardFront
        };
    }

    public CardStatsSO GetCardStatsSO() {
        return cardStatsSO;
    }
}
