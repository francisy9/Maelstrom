public static class Types
{
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
}
