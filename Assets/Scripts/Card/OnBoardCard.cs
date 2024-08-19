using System;
using UnityEngine;
using static Types;

public class OnBoardCard : CanAttackBase
{
    [SerializeField] private OnBoardCardVisual onBoardCardVisual;
    private int uid;

    public void InitCard(CardStats cardStats, int cardUid) {
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


    // Updates current stats then returns true if supposed to be destroyed
    public bool UpdateSelf(CardStats cardStats) {
        this.cardStats = cardStats;
        if (this.cardStats.CurrentHP <= 0) {
            DestroySelf();
            return true;
        }
        return false;
    }
}
