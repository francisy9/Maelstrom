using UnityEngine;
using System.Collections.Generic;
using System;
using CardTypes;

public abstract class BoardBase : MonoBehaviour
{
    // boardIndex -> OnBoardCard
    public List<OnBoardCard> onBoardCards;
    // cardUid -> boardIndex
    public Dictionary<int, int> boardIndexHashMap;
    public HashMapVisItem[] hashMapVis;
    [SerializeField] private GameObject onBoardCardObject;
    [SerializeField] private BoardBase enemyBoard;
    private int cardUid;

    [Serializable]
    public struct HashMapVisItem {
        public int cUid;
        public int boardIndex;
    }

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
        List<HashMapVisItem> hashMapVisItems = new List<HashMapVisItem>();
        foreach (Transform child in transform) {
            OnBoardCard card = child.GetComponent<OnBoardCard>();
            newBoardIndexHashMap.Add(card.GetCardUid(), child.GetSiblingIndex());
            HashMapVisItem hashMapVisItem = new()
            {
                cUid = card.GetCardUid(),
                boardIndex = child.GetSiblingIndex(),
            };
            hashMapVisItems.Add(hashMapVisItem);
        }
        boardIndexHashMap = newBoardIndexHashMap;
        hashMapVis = hashMapVisItems.ToArray();
    }

    public void UpdateCardAfterAttack(int boardIndex, UnitCardStats cardStats, bool removeDeadUnits = true) {
        OnBoardCard card = onBoardCards[boardIndex];

        Debug.Log($"Updating card at {cardStats.CardName} with {cardStats.CurrentHP} hp left");
        card.UpdateSelf(cardStats);
        Debug.Log($"Card now has {cardStats.CurrentHP} hp");

        if (cardStats.CurrentHP <= 0 && removeDeadUnits) {
            RemoveDeadUnit(card);
        }
    }

    public void RemoveDeadUnits() {
        for (int i = onBoardCards.Count - 1; i >= 0; i--) {
            OnBoardCard card = onBoardCards[i];
            if (card.GetRemainingHp() <= 0) {
                RemoveDeadUnit(card);
            }
        }
    }

    private void RemoveDeadUnit(OnBoardCard card) {
        int cardUid = card.GetCardUid();
        int boardIndex = GetBoardIndex(cardUid);
        RemoveFromHashMap(cardUid);
        onBoardCards[boardIndex].transform.SetParent(null);
        onBoardCards.RemoveAt(boardIndex);
        card.DestroySelf();
        UpdateBoardIndexHashMap();
    }

    // Distinction between the two for animation purposes
    public void CardAttack(int boardIndex, int opponentBoardIndex, object attackerStats, object targetStats) {
        bool attackerIsCard = GameManager.Instance.IsValidBoardIndexForCard(boardIndex);
        bool targetIsCard = GameManager.Instance.IsValidBoardIndexForCard(opponentBoardIndex);

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
        bool targetIsCard = GameManager.Instance.IsValidBoardIndexForCard(boardIndex);
        bool attackerIsCard = GameManager.Instance.IsValidBoardIndexForCard(opponentBoardIndex);

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

    public Vector3 GetBoardPositionByCardIndex(int boardIndex) {
        return onBoardCards[boardIndex].transform.position;
    }
}
