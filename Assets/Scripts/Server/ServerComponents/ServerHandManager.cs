using System.Collections;
using System.Collections.Generic;
using CardTypes;
using Mirror;
using UnityEngine;

public class ServerHandManager : NetworkBehaviour
{
    [SerializeField] private List<BaseCardSO> p1Deck;
    [SerializeField] private List<BaseCardSO> p2Deck;

    [Server]
    // Note: player turns needs to be set beforehand
    public void StartGameDrawCards()
    {
        Player starter = GameManager.Instance.GetInTurnPlayer();
        Player nextPlayer = GameManager.Instance.GetNextPlayer();
        int numCardsToBeDrawnByFirstPlayer = 3;
        int numCardsToBeDrawnBySecondPlayer = 4;
        DrawCards(starter, numCardsToBeDrawnByFirstPlayer);
        DrawCards(nextPlayer, numCardsToBeDrawnBySecondPlayer);
    }

    [Server]
    public void DrawCards(Player player, int numCards) {
        for (int i = 0; i < numCards; i++) {
            ServerAddCardToHand(player);
        }
    }

    [Server]
    private void ServerAddCardToHand(Player player) {
        BaseCard baseCard = ServerDrawBaseCard(player);
        ServerState.Instance.AddCardToHand(baseCard, player);
        player.TargetAddCardToHand(GetSerializedBaseCard(baseCard));
        GameManager.Instance.GetOpposingPlayer(player).TargetAddCardToOpponentHand();
    }

    [Server]
    private BaseCard ServerDrawBaseCard(Player player) {
        List<BaseCardSO> deckToDrawFrom = player == GameManager.Instance.GetPlayerOne() ? p1Deck : p2Deck;
        if (deckToDrawFrom.Count == 0) {
            return null;
        }
        int cardIndex = Random.Range(0, deckToDrawFrom.Count);
        BaseCardSO cardDrawn = deckToDrawFrom[cardIndex];
        deckToDrawFrom.RemoveAt(cardIndex);

        return GetBaseCardFromSO(cardDrawn);
    }

    [Server]
    public BaseCard GetBaseCardFromSO(BaseCardSO baseCardSO) {
        switch (baseCardSO.CardType)
        {
            case CardType.Unit:
                UnitCardStats cardStats = new UnitCardStats(baseCardSO as UnitCardSO);
                return cardStats;
            case CardType.Spell:
                return new SpellCardStats(baseCardSO as SpellCardSO);
            default:
                Debug.LogError("unimplemented card type");
                break;
        }
        return new BaseCard();
    }

    private byte[] GetSerializedBaseCard(BaseCard baseCard) {
        switch (baseCard.CardType)
        {
            case CardType.Unit:
                return (baseCard as UnitCardStats).Serialize();
            case CardType.Spell:
                return (baseCard as SpellCardStats).Serialize();
            case CardType.Weapon:
                return (baseCard as WeaponCardStats).Serialize();
            default:
                Debug.LogError("unimplemented card type");
                break;
        }
        return new byte[0];
    }
}
