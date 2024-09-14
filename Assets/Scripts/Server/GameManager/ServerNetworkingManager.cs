using Mirror;
using UnityEngine;
using CardTypes;
using ResponseTypes;
using Animation;
using System.Collections.Generic;
using System;

public class ServerNetworkingManager : NetworkBehaviour
{
    // Commands to be called by client
    [Command(requiresAuthority = false)]
    public void CmdEndTurn(NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        ValidateRequest(requestingPlayer);
        GameManager.Instance.GetTurnManager().EndTurn(requestingPlayer);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayUnitCard(int handIndex, int boardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        ValidateRequest(requestingPlayer);

        GameManager.Instance.GetHandManager().PlayUnitCard(requestingPlayer, handIndex, boardIndex);
    }

    [Command(requiresAuthority = false)]
    public void CmdAttack(int boardIndex, int opponentBoardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        ValidateRequest(requestingPlayer);
        Debug.Log($"{requestingPlayer.name} is requesting to attack card own board index: {boardIndex} opponent board index: {opponentBoardIndex}");
        GameManager.Instance.GetBoardManager().Attack(requestingPlayer, boardIndex, opponentBoardIndex);
    }

    [Command(requiresAuthority = false)]
    public void CmdCastSpell(int handIndex, Targeting targeting, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        
        if (targeting == null) {
            Debug.Log($"{requestingPlayer.name} is requesting to cast spell at hand index: {handIndex} targeting null");
        } else {
            Debug.Log($"{requestingPlayer.name} is requesting to cast spell at hand index: {handIndex} targeting {targeting.targetType} index: {targeting.targetBoardIndices}");
        }

        ValidateRequest(requestingPlayer);
        SpellCardStats cardToBeCast = ServerState.Instance.GetCardStatsAtHandIndex(handIndex, requestingPlayer) as SpellCardStats;
        ServerState.Instance.CastSpell(handIndex, targeting, requestingPlayer);
    }

    // Server methods
    [Server]
    public void ServerPlayUnitCard(Player requestingPlayer, int handIndex, int boardIndex) {
        requestingPlayer.TargetPlayCard(handIndex, boardIndex);
    }

    [Server]
    public void ServerOpponentPlayUnitCard(Player requestingPlayer, byte[] cardData, int handIndex, int boardIndex) {
        GameManager.Instance.GetOpposingPlayer(requestingPlayer).TargetOpponentPlayCard(cardData, handIndex, boardIndex);
    }

    [Server]
    public void ServerAttack(Player requestingPlayer, int boardIndex, int opponentBoardIndex, byte[] attackerData, byte[] targetData) {
        requestingPlayer.TargetAttackResponse(boardIndex, opponentBoardIndex, attackerData, targetData);
        GameManager.Instance.GetOpposingPlayer(requestingPlayer).TargetOpponentAttackResponse(opponentBoardIndex, boardIndex, targetData, attackerData);
    }

    [Server]
    public void ServerSendCardEffectResponse(Player player, CardEffectResponse response) {
        player.TargetExecuteCardEffect(response.Serialize(true));
        GameManager.Instance.GetOpposingPlayer(player).TargetExecuteOpponentCardEffect(response.Serialize(false));
    }

    // Helper methods
    private void ValidateRequest(Player requestingPlayer) {
        if (!GameManager.Instance.GetTurnManager().IsPlayerInTurn(requestingPlayer)) {
            Debug.LogError("Player isn't in turn");
        }
    }
}
