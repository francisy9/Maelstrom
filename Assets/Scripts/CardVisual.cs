using System;
using TMPro;
using UnityEngine;
using static Types;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private DragCardController dragCardController;
    [SerializeField] private Card card;
    [SerializeField] private GameObject cardBackVisual;
    [SerializeField] private GameObject cardFrontVisual;
    [SerializeField] private GameObject onBoardVisual;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI onBoardAttackText;
    [SerializeField] private TextMeshProUGUI onBoardHpText;
    private CardStatsSO cardStatsSO;
    

    public void InitVisual() {
        cardStatsSO = GetComponentInParent<Card>().GetCardStatsSO();
        cardName.text = cardStatsSO.cardName;
        description.text = cardStatsSO.description;
        manaText.text = cardStatsSO.manaCost.ToString();
        attackText.text = cardStatsSO.attack.ToString();
        onBoardAttackText.text = cardStatsSO.attack.ToString();
        onBoardHpText.text = cardStatsSO.hp.ToString();
        hpText.text = cardStatsSO.hp.ToString();
        cardBackVisual.SetActive(false);
        cardFrontVisual.SetActive(true);
        onBoardVisual.SetActive(false);
        dragCardController.OnCardPlayed += DragCardController_OnCardPlayed;
        card.OnAttack += Card_OnAttack;
        card.OnBeingAttacked += Card_OnBeingAttacked;
    }

    public void UpdateBoardVisual(int remainingHp, int attackVal) {
        Debug.Log(remainingHp);
        hpText.text = remainingHp.ToString();
        attackText.text = attackVal.ToString();
    }

    public void SetCardVisualState(CardState cardState) {
        cardBackVisual.SetActive(false);
        cardFrontVisual.SetActive(false);
        onBoardVisual.SetActive(false);
        switch (cardState) {
            case CardState.CardBack:
                cardBackVisual.SetActive(true);
                break;
            case CardState.CardFront:
                cardFrontVisual.SetActive(true);
                break;
            case CardState.OnBoard:
                onBoardVisual.SetActive(true);
                break;
        }   
    }

    private void DragCardController_OnCardPlayed(object sender, EventArgs e)
    {
        SetCardVisualState(CardState.OnBoard);
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
