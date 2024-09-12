using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

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
}
