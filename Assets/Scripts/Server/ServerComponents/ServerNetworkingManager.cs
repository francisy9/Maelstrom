using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ServerNetworkingManager : NetworkBehaviour
{
    [Command(requiresAuthority = false)]
    public void CmdEndTurn(NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        if (GameManager.Instance.GetTurnManager().IsPlayerInTurn(requestingPlayer)) {
            GameManager.Instance.GetTurnManager().EndTurn(requestingPlayer);
        } else {
            Debug.LogError("Wrong player was able to request end turn");
        }
    }

    
}
