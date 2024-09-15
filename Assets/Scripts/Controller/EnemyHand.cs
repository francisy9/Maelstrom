using System;
using UnityEngine;
using CardTypes;

public class EnemyHand : MonoBehaviour
{
    [SerializeField] private GameObject cardBackObject;
    [SerializeField] private EnemyBoard enemyBoard;
    public event EventHandler OnCardDrawn;
    public event EventHandler OnCardPlayed;
    private int numCards;
    private Player player;

    public void SetPlayer(Player player) {
        this.player = player;
    }
    public void AddCardToHand() {
        numCards += 1;
        Instantiate(cardBackObject, transform);
        OnCardDrawn.Invoke(this, EventArgs.Empty);
    }

    public void PlayUnitCard(byte[] cardData, int handIndex, int boardIndex) {
        BaseCard baseCard = BaseCard.Deserialize(cardData);
        switch (baseCard.GetCardType())
        {
            case CardType.Unit:
                enemyBoard.PlaceCardOnBoard(baseCard as UnitCardStats, boardIndex, player);
                break;
            default:
                Debug.LogError("Card type not implemented");
                break;
        }
        DestroyCardAtIndex(handIndex);
        OnCardPlayed.Invoke(this, EventArgs.Empty);
        return;
    }

    public void PlaySpellCard(int handIndex) {
        DestroyCardAtIndex(handIndex);
        OnCardPlayed.Invoke(this, EventArgs.Empty);
        return;
    }

    public int GetNumCards() {
        return numCards;
    }

    private void DestroyCardAtIndex(int index) {
        int i = 0;
        foreach(Transform child in transform) {
            if (i == index) {
                Destroy(child.gameObject);
                return;
            }
            i++;
        }
    }
}
