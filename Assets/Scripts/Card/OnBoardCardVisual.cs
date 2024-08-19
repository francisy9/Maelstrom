using System;
using TMPro;
using UnityEngine;
using static Types;

public class OnBoardCardVisual : MonoBehaviour
{
    [SerializeField] private OnBoardCard card;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI hpText;
    private CardStats cardStats;
    

    public void InitVisual() {
        cardStats = card.GetCardStats();
        attackText.text = cardStats.CurrentAttack.ToString();
        hpText.text = cardStats.CurrentHP.ToString();

        card.OnCardStatsUpdate += UpdateVisualStats;
    }

    private void UpdateVisualStats(object sender, EventArgs e) {
        int remainingHp = card.GetRemainingHp();
        int attackVal = card.GetAttackVal();
        hpText.text = remainingHp.ToString();
        attackText.text = attackVal.ToString();

        if (remainingHp < cardStats.BaseHP) {
            hpText.color = Color.red;
        }

        if (remainingHp > cardStats.BaseHP) {
            hpText.color = Color.green;
        }

    }
 }
