using System;
using TMPro;
using UnityEngine;
using static Types;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private DragCardController cardController;
    [SerializeField] private GameObject cardBackVisual;
    [SerializeField] private GameObject cardFrontVisual;
    [SerializeField] private GameObject onBoardVisual;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private DropZone dropZone;
    [SerializeField] private TextMeshProUGUI onBoardAttackText;
    [SerializeField] private TextMeshProUGUI onBoardHpText;
    

    private void Start() {
        CardStatsSO cardStatsSO = GetComponentInParent<Card>().GetCardStatsSO();
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
        cardController.OnCardPlayed += CardController_OnCardPlayed;
    }

    public void UpdateVisual(CardStats cardStats) {
        hpText.text = cardStats.Hp.ToString();
        manaText.text = cardStats.Mana.ToString();
        attackText.text = cardStats.Attack.ToString();
        SetCardVisualState(cardStats.CardState);
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

    private void CardController_OnCardPlayed(object sender, EventArgs e)
    {
        SetCardVisualState(CardState.OnBoard);
    }
 }
