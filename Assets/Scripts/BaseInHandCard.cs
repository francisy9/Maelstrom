using UnityEngine;
using static Types;

public class BaseInHandCard : MonoBehaviour
{
    [SerializeField] private GameObject unitComponents;
    [SerializeField] private GameObject spellComponents;
    [SerializeField] private GameObject weaponComponents;

    public void SelectCardVariant(CardType cardType) {
        switch(cardType) {
            case CardType.Unit:
                unitComponents.SetActive(true);
                spellComponents.SetActive(false);
                weaponComponents.SetActive(false);
                break;
            case CardType.Weapon:
                weaponComponents.SetActive(true);
                unitComponents.SetActive(false);
                spellComponents.SetActive(false);
                break;
            case CardType.Spell:
                spellComponents.SetActive(true);
                weaponComponents.SetActive(false);
                unitComponents.SetActive(false);
                break;
        }
    }
}
