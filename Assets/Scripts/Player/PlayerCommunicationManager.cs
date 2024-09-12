using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using CardTypes;

public class PlayerCommunicationManager : NetworkBehaviour
{
    private Player player;

    public void Initialize(Player player) {
        this.player = player;
    }

    [TargetRpc]
    public void TargetBeginGame(bool first) {}
    
    [TargetRpc]
    public void TargetInitializeHeroes(int heroStartHp, int enemyHeroStartHp) {
        player.GetHeroManager().InitHeroes(heroStartHp, enemyHeroStartHp);
    }

    [TargetRpc]
    public void TargetAddCardToHand(byte[] serializedCardArray) {
        BaseCard baseCard = BaseCard.Deserialize(serializedCardArray);
        player.GetHandManager().AddCardToHand(baseCard);
    }

    [TargetRpc]
    public void TargetAddCardToOpponentHand() {
        player.GetHandManager().AddCardToOpponentHand();
    }

    [TargetRpc]
    public void TargetPlayCard(int handIndex, int boardIndex) {
        player.GetHandManager().PlayCard(handIndex, boardIndex);
    }

    [TargetRpc]
    public void TargetOpponentPlayCard(byte[] cardData, int handIndex, int boardIndex) {
        player.GetHandManager().OpponentPlayCard(cardData, handIndex, boardIndex);
    }

    internal void RequestPlayCard(int handIndex, int boardIndex)
    {
        GameManager.Instance.CmdPlayUnitCard(handIndex, boardIndex);
        return;
    }

    [TargetRpc]
    public void TargetAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) {
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        object attackerStats;
        object targetStats;

        if (attackerIsCard) {
            attackerStats = UnitCardStats.Deserialize(cardData);
        } else {
            attackerStats = HeroStats.Deserialize(cardData);
        }

        if (targetIsCard) {
            targetStats = UnitCardStats.Deserialize(opponentCardData);
        } else {
            targetStats = HeroStats.Deserialize(opponentCardData);
        }

        player.GetBoardManager().CardAttack(boardIndex, opponentBoardIndex, attackerStats, targetStats);
    }

    [TargetRpc]
    public void TargetOpponentAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData) {
        // Note the flip in boardIndex and opponentBoardIndex
        // attacker is the opponent
        // self unit is target
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        object targetStats;
        object attackerStats;

        if (targetIsCard) {
            targetStats = UnitCardStats.Deserialize(cardData);
        } else {
            targetStats = HeroStats.Deserialize(cardData);
        }

        if (attackerIsCard) {
            attackerStats = UnitCardStats.Deserialize(opponentCardData);
        } else {
            attackerStats = HeroStats.Deserialize(opponentCardData);
        }

        player.GetBoardManager().CardAttackedBy(boardIndex, opponentBoardIndex, targetStats, attackerStats);
    }

    internal void RequestAttack(int boardIndex, int opponentBoardIndex)
    {
        GameManager.Instance.CmdAttack(boardIndex, opponentBoardIndex);
        return;
    }

    internal void RequestCastSpell(int handIndex, Targeting targeting) {
        GameManager.Instance.CmdCastSpell(handIndex, targeting);
        return;
    }
}
