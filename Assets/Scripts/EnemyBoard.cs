using System.Collections.Generic;
using UnityEngine;
using static Types;

public class EnemyBoard : BoardBase
{
    public static EnemyBoard Instance;

    public override void Awake() {
        base.Awake();
        Instance = this;
    }

    public override void PlaceCardOnBoard(CardStats cardStats, int boardIndex) {
        base.PlaceCardOnBoard(cardStats, boardIndex);
        onBoardCards[boardIndex].gameObject.layer = LayerMask.NameToLayer(OPPONENT_PLAYED_CARD_LAYER);
        UpdateBoardIndexHashMap();
    }
}
