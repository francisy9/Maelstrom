using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private CardStatsSO cardStatsSO;
    [SerializeField] private GameObject cardBackVisual;
    [SerializeField] private GameObject cardFrontVisual;
    [SerializeField] private GameObject onBoardVisual;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI hpText;
    

    private void Start() {
        cardName.text = cardStatsSO.cardName;
        description.text = cardStatsSO.description;
        manaText.text = cardStatsSO.manaCost.ToString();
        attackText.text = cardStatsSO.attack.ToString();
        hpText.text = cardStatsSO.hp.ToString();
        cardBackVisual.SetActive(false);
        cardFrontVisual.SetActive(true);
        onBoardVisual.SetActive(false);
    }

    public void UpdateVisual(Card.CardStats cardStats) {
        hpText.text = cardStats.Hp.ToString();
        manaText.text = cardStats.Mana.ToString();
        attackText.text = cardStats.Attack.ToString();


        cardBackVisual.SetActive(false);
        cardBackVisual.SetActive(false);
        cardBackVisual.SetActive(false);
        switch (cardStats.CardState) {
            case Card.CardState.CardBack:
                cardBackVisual.SetActive(true);
                break;
            case Card.CardState.CardFront:
                cardFrontVisual.SetActive(true);
                break;
            case Card.CardState.OnBoard:
                onBoardVisual.SetActive(true);
                break;
        }   
        
    }
}
