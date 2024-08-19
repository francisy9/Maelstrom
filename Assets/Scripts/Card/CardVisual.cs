using System;
using TMPro;
using UnityEngine;
using static Types;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI hpText;
    private CardStats cardStats;
    

    public void InitVisual() {
        cardStats = card.GetCardStats();
        cardName.text = cardStats.CardName;
        description.text = cardStats.Description;
        manaText.text = cardStats.CurrentManaCost.ToString();
        attackText.text = cardStats.CurrentAttack.ToString();
        hpText.text = cardStats.MaxHP.ToString();
    }
 }
