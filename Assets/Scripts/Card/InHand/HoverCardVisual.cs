using TMPro;
using UnityEngine;
using static Types;

public class CardVisualPlaceHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI hpText;
    

    public void InitVisual(string name, string description, int mana, int attack, int hp) {
        cardName.text = name;
        this.description.text = description;
        manaText.text = mana.ToString();
        attackText.text = attack.ToString();
        hpText.text = hp.ToString();
    }
 }
