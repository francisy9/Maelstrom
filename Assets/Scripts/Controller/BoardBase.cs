using UnityEngine;
using static Types;
using System.Collections.Generic;

public abstract class BoardBase : MonoBehaviour
{
    // boardIndex -> OnBoardCard
    public List<OnBoardCard> onBoardCards;
    // cardUid -> boardIndex
    public Dictionary<int, int> boardIndexHashMap;
    [SerializeField] private GameObject onBoardCardObject;
    [SerializeField] private BoardBase enemyBoard;
    private int cardUid;

    public virtual void Awake() {
        onBoardCards = new List<OnBoardCard>();
        boardIndexHashMap = new Dictionary<int, int>();
        cardUid = 0;
    }


    public virtual void PlaceCardOnBoard(UnitCardStats cardStats, int boardIndex, Player player) {
        GameObject cardObject = Instantiate(onBoardCardObject, transform);
        cardObject.transform.SetSiblingIndex(boardIndex);
        OnBoardCard cardComponent = cardObject.GetComponent<OnBoardCard>();
        cardComponent.InitCard(cardStats, cardUid, player);
        onBoardCards.Insert(boardIndex, cardComponent);
        boardIndexHashMap.Add(cardUid, boardIndex);
        cardUid += 1;
    }

    public int GetBoardIndex(int cardUid) {
        if (!boardIndexHashMap.ContainsKey(cardUid)) {
            Debug.LogError($"boardIndexHashMap doesn't have carduid: {cardUid}");
        }
        return boardIndexHashMap[cardUid];
    }

    public void UpdateBoardIndexHashMap() {
        Dictionary<int, int> newBoardIndexHashMap = new Dictionary<int, int>();
        foreach (Transform child in transform) {
            OnBoardCard card = child.GetComponent<OnBoardCard>();
            newBoardIndexHashMap.Add(card.GetCardUid(), child.GetSiblingIndex());
        }
        boardIndexHashMap = newBoardIndexHashMap;
    }

    public void UpdateCardAfterAttack(int boardIndex, UnitCardStats cardStats) {
        if (onBoardCards[boardIndex].UpdateSelf(cardStats)) {
            int cardUid = onBoardCards[boardIndex].GetCardUid();
            RemoveFromHashMap(cardUid);
            onBoardCards[boardIndex].transform.SetParent(null);
            onBoardCards.RemoveAt(boardIndex);
            UpdateBoardIndexHashMap();
        };
    }

    // Distinction between the two for animation purposes
    public void CardAttack(int boardIndex, int opponentBoardIndex, object attackerStats, object targetStats) {
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        if (attackerIsCard) {
           UpdateCardAfterAttack(boardIndex, attackerStats as UnitCardStats);
        } else {
            Hero.Instance.UpdateSelf(attackerStats as HeroStats);
        }

        if (targetIsCard) {
            enemyBoard.UpdateCardAfterAttack(opponentBoardIndex, targetStats as UnitCardStats);
        } else {
            EnemyHero.Instance.UpdateSelf(targetStats as HeroStats);
        }
    }

    public void CardAttackedBy(int boardIndex, int opponentBoardIndex, object targetStats, object attackerStats) {
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        if (targetIsCard) {
            UpdateCardAfterAttack(boardIndex, targetStats as UnitCardStats);
        } else {
            Hero.Instance.UpdateSelf(targetStats as HeroStats);
        }

        if (attackerIsCard) {
           enemyBoard.UpdateCardAfterAttack(opponentBoardIndex, attackerStats as UnitCardStats);
        } else {
            EnemyHero.Instance.UpdateSelf(attackerStats as HeroStats);
        }
    }

    public void RemoveFromHashMap(int cardUid) {
        boardIndexHashMap.Remove(cardUid);
    }
}
