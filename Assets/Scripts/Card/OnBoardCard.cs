using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class OnBoardCard : CanAttackBase
{
    [SerializeField] private OnBoardCardVisual onBoardCardVisual;
    private int uid;
    public event EventHandler OnCardStatsUpdate;
    public UnitCardStats cardStats;


    public void InitCard(UnitCardStats cardStats, int cardUid, Player player) {
        base.InitCanAttackBase(player);
        this.cardStats = cardStats;
        onBoardCardVisual.InitVisual();
        uid = cardUid;
    }

    public override void Death() {
        // OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public int GetCardUid() {
        return uid;
    }

    public UnitCardStats GetCardStats() {
        return cardStats;
    }

    public override int GetRemainingHp() {
        return cardStats.CurrentHP;
    }

    public override int GetAttackVal() {
        return cardStats.CurrentAttack;
    }

    public override int GetRemainingNumAttacks() {
        return cardStats.NumAttacks;
    }

    public override void ResetAttack() {
        cardStats.NumAttacks = cardStats.TotalNumAttacks;
    }

    // Updates current stats then returns true if supposed to be destroyed
    public bool UpdateSelf(UnitCardStats cardStats) {
        this.cardStats = cardStats;
        OnCardStatsUpdate.Invoke(this, EventArgs.Empty);
        if (this.cardStats.CurrentHP <= 0) {
            DestroySelf();
            return true;
        }
        return false;
    }


    // Drag features
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!player.IsTurn()) {
            Debug.Log("Not your turn");
            eventData.pointerDrag = null;
            return;
        }


        if (!CanAttack()) {
            eventData.pointerDrag = null;
            return;
        }

        Vector3 attackFromVec3 = new Vector3(transform.position.x, transform.position.y, -1);
        Vector3 attackToVec3 = new Vector3(eventData.position.x, eventData.position.y, -1);
        LineController.Instance.Show();
        LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);

    }

    public override void OnDrag(PointerEventData eventData)
    {
        currentlyDetectedTarget = DetectTarget(eventData);
        Vector3 attackFromVec3 = new Vector3(transform.position.x, transform.position.y, -1);
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

        if (currentlyDetectedTarget != null) {
            pointerWorldPos = currentlyDetectedTarget.transform.position;
        } else {
            currentlyDetectedTarget = null;
        }

        Vector3 attackToVec3 = new Vector3(pointerWorldPos.x, pointerWorldPos.y, -1);
        LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"On end drag target is not null: {currentlyDetectedTarget != null}");
        if (currentlyDetectedTarget) {
            RequestAttack();
        }
        LineController.Instance.Hide();
    }
}
