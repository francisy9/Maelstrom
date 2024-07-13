using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardStatsSO cardStatsSO;
    public CardStats cardStats;
    
    public enum CardState {
        CardBack,
        CardFront,
        OnBoard,
    }

    public class CardStats
    {
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Mana { get; set; }
        public CardState CardState { get; set; }
    }

    private void Awake() {
        cardStats = new CardStats();
        cardStats.Hp = cardStatsSO.hp;
        cardStats.Attack = cardStatsSO.attack;
        cardStats.Mana = cardStatsSO.manaCost;
        cardStats.CardState = CardState.CardFront;
    }


}
