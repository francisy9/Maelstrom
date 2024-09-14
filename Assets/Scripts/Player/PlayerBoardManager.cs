using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using ResponseTypes;
using UnityEngine;
using CardTypes;
using static Constants.Constants;

public class PlayerBoardManager : NetworkBehaviour
{
    private Player player;
    [SerializeField] private Board board;
    [SerializeField] private EnemyBoard enemyBoard;

    public void Initialize(Player player) {
        this.player = player;
        board.SetPlayer(player);
    }

    public Board GetBoard() {
        return board;
    }

    public EnemyBoard GetEnemyBoard() {
        return enemyBoard;
    }

    public void CardAttack(int boardIndex, int opponentBoardIndex, object attackerStats, object targetStats) {
        board.CardAttack(boardIndex, opponentBoardIndex, attackerStats, targetStats);
    }

    public void CardAttackedBy(int boardIndex, int opponentBoardIndex, object attackerStats, object targetStats) {
        board.CardAttackedBy(boardIndex, opponentBoardIndex, attackerStats, targetStats);
    }

    public Vector3 GetUnitPosition(TargetType targetType, int boardIndex) {
        Debug.Log($"Getting unit position from board {targetType} for {boardIndex}");
        if (boardIndex == HERO_BOARD_INDEX) {
            return targetType == TargetType.Ally ? player.GetHeroManager().GetHeroPosition() : player.GetHeroManager().GetEnemyHeroPosition();
        }
        BoardBase queryingBoard = targetType == TargetType.Ally ? board as BoardBase : enemyBoard as BoardBase;
        return queryingBoard.GetBoardPositionByCardIndex(boardIndex);
    }

    internal Vector3[] GetAffectedUnitPositions(CardEffectResponse response)
    {
        List<Vector3> affectedUnitPositions = new List<Vector3>();
        if (response.AlliedUnits != null) {
            foreach (KeyValuePair<int, byte[]> kvp in response.AlliedUnits) {
                affectedUnitPositions.Add(GetUnitPosition(TargetType.Ally, kvp.Key));
            }
        }
        if (response.EnemyUnits != null) {
            foreach (KeyValuePair<int, byte[]> kvp in response.EnemyUnits) {
                affectedUnitPositions.Add(GetUnitPosition(TargetType.Enemy, kvp.Key));
            }
        }
        return affectedUnitPositions.ToArray();
    }
}
