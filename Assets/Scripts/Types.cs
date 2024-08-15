public static class Types
{
    public enum CardState {
        CardBack,
        CardFront,
        OnBoard,
    }

    public enum CardLocation {
        Hand,
        Board,
        Discard,
    }

    public class InPlayStats {
        public int attacks;
        public int totalAttacks;
        public int attackVal;
        public int hp;

    }

    public static string PLAYER_CARD_LAYER = "PlayedCard";
    public static string OPPONENT_PLAYED_CARD_LAYER = "OpponentPlayedCard";
}
