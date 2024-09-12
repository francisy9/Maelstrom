using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Layers.Layers;
using static Const.Const;

public abstract class CanAttackBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Player player;
    public event EventHandler OnDeath;

    public abstract int GetRemainingHp();

    public abstract int GetAttackVal();

    public abstract int GetRemainingNumAttacks();
    public GameObject currentlyDetectedTarget;

    public virtual void InitCanAttackBase(Player player) {
        this.player = player;
    }


    public bool CanAttack() {
        if (GetAttackVal() <= 0) {
            Debug.Log("Unit's attack value is 0!");
            return false;
        }

        if (GetRemainingNumAttacks() <= 0) {
            Debug.Log("No attacks remaining");
            return false;
        }

        return true;
    }

    public abstract void ResetAttack();

    public abstract void Death();

    public abstract void OnBeginDrag(PointerEventData eventData);

    public abstract void OnDrag(PointerEventData eventData);

    public abstract void OnEndDrag(PointerEventData eventData);

    public GameObject DetectTarget(PointerEventData pointerEventData) {
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(pointerEventData.position);
        LayerMask opponentCardLayerMask = LayerMask.GetMask(OPPONENT_PLAYED_CARD_LAYER);
        LayerMask opponentHeroLayerMask = LayerMask.GetMask(OPPONENT_HERO_LAYER);

        RaycastHit2D hit = Physics2D.Raycast(pointerWorldPos, Vector2.zero, 1.0f, opponentCardLayerMask | opponentHeroLayerMask);
        if (hit.collider != null) {
            GameObject opponentCardGameObject = hit.collider.gameObject;
            return opponentCardGameObject;
        }
        return null;
    }

    public int GetSelfBoardIndex() {
        if (TryGetComponent<OnBoardCard>(out OnBoardCard card)) {
            return player.GetBoard().GetBoardIndex(card.GetCardUid());
        } else {
            // Caller is of hero type
            return HERO_BOARD_INDEX;
        }
    }

    public void RequestAttack() {
        int attackerBoardIndex;

        if (TryGetComponent<OnBoardCard>(out OnBoardCard card)) {
            attackerBoardIndex = player.GetBoard().GetBoardIndex(card.GetCardUid());
        } else {
            // Caller is of hero type
            attackerBoardIndex = HERO_BOARD_INDEX;
        }

        int targetBoardIndex;
        if (currentlyDetectedTarget.TryGetComponent<OnBoardCard>(out OnBoardCard enemyCard)) {
            targetBoardIndex = player.GetEnemyBoard().GetBoardIndex(enemyCard.GetCardUid());
        } else {
            // Target is of hero type
            targetBoardIndex = HERO_BOARD_INDEX;
        }
        
        player.RequestAttack(attackerBoardIndex, targetBoardIndex);
    }
}
