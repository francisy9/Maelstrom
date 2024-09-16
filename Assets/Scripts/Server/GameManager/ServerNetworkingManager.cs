using Mirror;
using UnityEngine;
using CardTypes;
using ResponseTypes;

public class ServerNetworkingManager : NetworkBehaviour
{
    // Commands to be called by client
    [Command(requiresAuthority = false)]
    public void CmdEndTurn(string requestId, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        ValidateAndAcknowledgeRequest(requestingPlayer, requestId);
        GameManager.Instance.GetTurnManager().EndTurn(requestingPlayer);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayUnitCard(string requestId, int handIndex, int boardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        ValidateAndAcknowledgeRequest(requestingPlayer, requestId);

        GameManager.Instance.GetHandManager().PlayUnitCard(requestingPlayer, handIndex, boardIndex);
    }

    [Command(requiresAuthority = false)]
    public void CmdAttack(string requestId, int boardIndex, int opponentBoardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        ValidateAndAcknowledgeRequest(requestingPlayer, requestId);
        Debug.Log($"{requestingPlayer.name} is requesting to attack card own board index: {boardIndex} opponent board index: {opponentBoardIndex}");
        GameManager.Instance.GetBoardManager().Attack(requestingPlayer, boardIndex, opponentBoardIndex);
    }

    [Command(requiresAuthority = false)]
    public void CmdCastSpell(string requestId, int handIndex, Targeting targeting, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        
        if (targeting == null) {
            Debug.Log($"{requestingPlayer.name} is requesting to cast spell at hand index: {handIndex} targeting null");
        } else {
            Debug.Log($"{requestingPlayer.name} is requesting to cast spell at hand index: {handIndex} targeting {targeting.targetType} index: {targeting.targetBoardIndices}");
        }

        ValidateAndAcknowledgeRequest(requestingPlayer, requestId);

        ServerState.Instance.CastSpell(handIndex, targeting, requestingPlayer);
    }

    // Server methods
    [Server]
    public void ServerPlayUnitCard(Player requestingPlayer, int handIndex, int boardIndex) {
        requestingPlayer.TargetPlayUnitCard(handIndex, boardIndex);
    }

    [Server]
    public void ServerOpponentPlayUnitCard(Player requestingPlayer, byte[] cardData, int handIndex, int boardIndex) {
        GameManager.Instance.GetOpposingPlayer(requestingPlayer).TargetOpponentUnitPlayCard(cardData, handIndex, boardIndex);
    }

    [Server]
    public void ServerPlaySpellCard(Player requestingPlayer, int handIndex) {
        requestingPlayer.TargetPlaySpellCard(handIndex);
        GameManager.Instance.GetOpposingPlayer(requestingPlayer).TargetOpponentPlaySpellCard(handIndex);
    }

    [Server]
    public void ServerAttack(Player requestingPlayer, int boardIndex, int opponentBoardIndex, byte[] attackerData, byte[] targetData) {
        requestingPlayer.TargetAttackResponse(boardIndex, opponentBoardIndex, attackerData, targetData);
        GameManager.Instance.GetOpposingPlayer(requestingPlayer).TargetOpponentAttackResponse(opponentBoardIndex, boardIndex, targetData, attackerData);
    }

    [Server]
    public void ServerSendCardEffectResponse(Player player, CardEffectResponse response) {
        ServerState.Instance.PrintServerGameState();
        player.TargetExecuteCardEffect(response.Serialize(true));
        GameManager.Instance.GetOpposingPlayer(player).TargetExecuteOpponentCardEffect(response.Serialize(false));
    }

    // Helper methods
    private void ValidateAndAcknowledgeRequest(Player requestingPlayer, string requestId) {
        if (!GameManager.Instance.GetTurnManager().IsPlayerInTurn(requestingPlayer)) {
            Debug.LogError("Player isn't in turn");
        }
        requestingPlayer.GetCommunicationManager().TargetAcknowledgeRequest(requestId);
    }
}
