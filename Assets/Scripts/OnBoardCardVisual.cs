using System;
using TMPro;
using UnityEngine;
using static Types;

public class OnBoardCardVisual : MonoBehaviour
{
    [SerializeField] private OnBoardCard card;
    [SerializeField] private TextMeshProUGUI onBoardAttackText;
    [SerializeField] private TextMeshProUGUI onBoardHpText;
    private CardStatsSO cardStatsSO;
    

    public void InitVisual() {
        cardStatsSO = GetComponentInParent<OnBoardCard>().GetCardStatsSO();
        onBoardAttackText.text = cardStatsSO.attack.ToString();
        onBoardHpText.text = cardStatsSO.hp.ToString();

        card.OnAttack += Card_OnAttack;
        card.OnBeingAttacked += Card_OnBeingAttacked;
    }

    public void UpdateBoardVisual(int remainingHp, int attackVal) {
        Debug.Log(remainingHp);
        onBoardHpText.text = remainingHp.ToString();
        onBoardAttackText.text = attackVal.ToString();
    }

    private void Card_OnAttack(object sender, EventArgs e)
    {
        int remainingHp = card.GetRemainingHp();
        onBoardHpText.text = remainingHp.ToString();
        if (remainingHp < cardStatsSO.hp) {
            onBoardHpText.color = Color.red;
        }
    }

    // Splitting into 2 separate functions now since it gets more complicated once 
    // more features are implemented
    private void Card_OnBeingAttacked(object sender, EventArgs e)
    {
        int remainingHp = card.GetRemainingHp();
        onBoardHpText.text = remainingHp.ToString();
        if (remainingHp < cardStatsSO.hp) {
            onBoardHpText.color = Color.red;
        }
    }
 }
