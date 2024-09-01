using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public abstract class HeroBase : CanAttackBase
{
    private HeroStats heroStats;
    public event EventHandler OnChange;
    [SerializeField] public HeroVisual heroVisual;

    public virtual void InitHero(int hp) {
        heroStats = new HeroStats
        {
            MaxHP = hp,
            CurrentHP = hp
        };
    }

    public int GetTotalHp() {
        return heroStats.MaxHP;
    }

    public override int GetRemainingHp() {
        return heroStats.CurrentHP;
    }

    public override int GetAttackVal() {
        return heroStats.CurrentAttack;
    }

    public override int GetRemainingNumAttacks() {
        return heroStats.NumAttacks;
    }

    public override void ResetAttack() {
        heroStats.NumAttacks = heroStats.TotalNumAttacks;
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
        GameObject target = DetectTarget(eventData);
        Vector3 attackFromVec3 = new Vector3(transform.position.x, transform.position.y, -1);
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

        if (target != null) {
            pointerWorldPos = target.transform.position;
        } else {
            currentlyDetectedTarget = null;
        }

        Vector3 attackToVec3 = new Vector3(pointerWorldPos.x, pointerWorldPos.y, -1);
        LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (currentlyDetectedTarget) {
            RequestAttack();
        }
        LineController.Instance.Hide();
    }

    public void UpdateSelf(HeroStats stats) {
        heroStats = stats;
        OnChange?.Invoke(this, EventArgs.Empty);
    }
}
