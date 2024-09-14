using Mirror;
using UnityEngine;
using CardTypes;
using static Constants.Constants;

public class ServerBoardManager : NetworkBehaviour
{
    [Server]
    public void Attack(Player requestingPlayer, int boardIndex, int opponentBoardIndex) {
        ValidateBoardIndex(boardIndex);
        ValidateBoardIndex(opponentBoardIndex);

        bool attackerIsCard;
        if (IsHeroAtBoardIndex(boardIndex)) {
            // Attacker is hero
            attackerIsCard = false;
            HeroStats hero = ServerState.Instance.GetHeroStats(requestingPlayer);

            if (hero.CurrentAttack <= 0) {
                Debug.LogError("Hero's attack value is 0!");
                return;
            }

            if (hero.NumAttacks <= 0) {
                Debug.LogError("Insufficient attacks remaining");
            }
        } else {
            // Attacker is card type
            attackerIsCard = true;
            UnitCardStats card = ServerState.Instance.GetCardStatsAtBoardIndex(boardIndex, requestingPlayer);

            if (card.CurrentAttack <= 0) {
                Debug.LogError("Unit's attack value is 0!");
                return;
            }

            if (card.NumAttacks <= 0) {
                Debug.LogError("Insufficient attacks remaining");
            }
        }

        byte[] attackerData;
        byte[] targetData;
        object[] serverUpdateResponses = ServerState.Instance.Attack(boardIndex, opponentBoardIndex, requestingPlayer);

        if (attackerIsCard) {
            attackerData = (serverUpdateResponses[0] as UnitCardStats).Serialize();
        } else {
            attackerData = (serverUpdateResponses[0] as HeroStats).Serialize();
        }

        if (IsHeroAtBoardIndex(opponentBoardIndex)) {
            targetData = (serverUpdateResponses[1] as HeroStats).Serialize();
        } else {
            targetData = (serverUpdateResponses[1] as UnitCardStats).Serialize();
        }

        GameManager.Instance.GetNetworkingManager().ServerAttack(requestingPlayer, boardIndex, opponentBoardIndex, attackerData, targetData);
        ServerState.Instance.PrintServerGameState();
    }

    private void ValidateBoardIndex(int boardIndex) {
        if (!IsValidBoardIndexForCard(boardIndex) && !IsHeroAtBoardIndex(boardIndex)) {
            Debug.LogError($"Invalid board index: {boardIndex}");
        }
    }

    public bool IsValidBoardIndexForCard(int boardIndex) {
        return boardIndex <= MAX_BOARD_INDEX && boardIndex >= 0;
    }

    private bool IsHeroAtBoardIndex(int boardIndex) {
        return boardIndex == HERO_BOARD_INDEX;
    }
}
