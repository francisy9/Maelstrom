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

    public enum CardLocation {
        Hand,
        Board,
        Discard,
    }

    public static string PLAYER_CARD_LAYER = "PlayedCard";
    public static string OPPONENT_PLAYED_CARD_LAYER = "OpponentPlayedCard";
}
