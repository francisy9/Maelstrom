using System;
using UnityEngine;
using static Types;

public class EnemyHand : MonoBehaviour
{
    [SerializeField] private GameObject cardBackObject;
    [SerializeField] private EnemyBoard enemyBoard;
    public event EventHandler OnCardDrawn;
    public event EventHandler OnCardPlayed;
    private int numCards;

    public void AddCardToHand() {
        numCards += 1;
        Instantiate(cardBackObject, transform);
        OnCardDrawn.Invoke(this, EventArgs.Empty);
    }

    public void PlayCard(byte[] cardData, int handIndex, int boardIndex) {
        CardStats cardStats = CardStats.Deserialize(cardData);
        enemyBoard.PlaceCardOnBoard(cardStats, boardIndex);
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
