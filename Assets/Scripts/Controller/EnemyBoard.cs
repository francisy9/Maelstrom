using System.Collections.Generic;
using UnityEngine;
using static Types;

public class EnemyBoard : BoardBase
{
    public override void Awake() {
        base.Awake();
    }

    public override void PlaceCardOnBoard(UnitCardStats cardStats, int boardIndex, Player _) {
        base.PlaceCardOnBoard(cardStats, boardIndex, _);
        onBoardCards[boardIndex].gameObject.layer = LayerMask.NameToLayer(OPPONENT_PLAYED_CARD_LAYER);
        UpdateBoardIndexHashMap();
    }
}
