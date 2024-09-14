using System.Collections;
using System.Collections.Generic;
using Mirror;
using CardTypes;

public class ServerHandStateManager : NetworkBehaviour
{
    private List<BaseCard> p1Hand;
    private List<BaseCard> p2Hand;

    [Server]
    public void Initialize() {
        p1Hand = new List<BaseCard>();
        p2Hand = new List<BaseCard>();
    }
    
    [Server]
    public void AddCardToHand(BaseCard baseCard, Player player) {
        GetHand(player).Add(baseCard);
    }

    [Server]
    public List<BaseCard> GetHand(Player player) {
        return player == ServerState.Instance.GetPlayerOne() ? p1Hand : p2Hand;
    }

    [Server]
    public BaseCard GetCardStatsAtHandIndex(int i, Player player) {
        return GetHand(player)[i];
    }
}
