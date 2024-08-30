using System;
using UnityEngine;

public class EnemyHandVisual : MonoBehaviour
{
    public static EnemyHandVisual Instance;
    private float width;
    private int numCards;
    private RectTransform rectTransform;
    private float fanAngle;
    [SerializeField] private float fanAngleMin;
    [SerializeField] private float fanAngleMax;
    [SerializeField] private float YOffset;
    private float angleStep;
    private EnemyHand enemyHand;

    private void Awake() {
        rectTransform = transform as RectTransform;
        width = rectTransform.rect.width;
        Instance = this;
    }
    
    private void Start() {
        enemyHand = GetComponent<EnemyHand>();
        enemyHand.OnCardDrawn += EnemyHand_CardAddedToHand;
        enemyHand.OnCardPlayed += EnemyHand_OnCardPlayed;
    }

    private void EnemyHand_CardAddedToHand(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void EnemyHand_OnCardPlayed(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        numCards = enemyHand.GetNumCards();
        int i  = 0;
        fanAngle = GetFanAngle();

        if (numCards > 1) {
            angleStep = fanAngle * 2 / (numCards - 1);
        } else {
            angleStep = fanAngle;
            i = 1;
        }

        foreach(Transform child in transform) {
            ArrangeCard(i, child);
            i++;
        }
        return;
    }

    private float GetFanAngle(){
        return Mathf.Lerp(fanAngleMin, fanAngleMax, (float) numCards/10);
    } 

    private void ArrangeCard(int cardIndex, Transform cardTransform) {
        float angle = fanAngle - angleStep * cardIndex;
        float rad = angle * Mathf.Deg2Rad;
        float yOffset = YOffset;

        float x = width * Mathf.Sin(rad);
        float y = width * Mathf.Cos(rad) - yOffset;

        cardTransform.SetLocalPositionAndRotation(new Vector3(-x , -y, 0), Quaternion.Euler(0, 0, -angle));
        return;
    }
}