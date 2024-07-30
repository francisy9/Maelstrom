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

    public static string PLAYER_CARD_LAYER = "PlayedCard";
    public static string OPPONENT_PLAYED_CARD_LAYER = "OpponentPlayedCard";
}
