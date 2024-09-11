using UnityEngine;
using TMPro;
using System;
using CardTypes;

public class InHandCardVisualBase : MonoBehaviour
{
    protected BaseCard cardStats;
    protected CardType cardType;
    protected CanvasGroup canvasGroup;

    [SerializeField] protected InHandCard card;
    [SerializeField] protected TextMeshProUGUI cardName;
    [SerializeField] protected TextMeshProUGUI manaText;
    [SerializeField] protected TextMeshProUGUI unitDescription;
    [SerializeField] protected TextMeshProUGUI spellDescription;
    [SerializeField] protected TextMeshProUGUI weaponDescription;
    [SerializeField] protected GameObject unitComponents;
    [SerializeField] protected GameObject spellComponents;
    [SerializeField] protected GameObject weaponComponents;
    [SerializeField] protected TextMeshProUGUI unitAttackText;
    [SerializeField] protected TextMeshProUGUI unitHpText;
    [SerializeField] protected TextMeshProUGUI weaponAttackText;
    [SerializeField] protected TextMeshProUGUI weaponDurabilityText;

    // Optional parameter used for hover card visual
    public virtual void InitVisual(BaseCard baseCard = null)
    {
        if (baseCard == null)
        {
            baseCard = card.GetCardStats();
        }
        cardStats = baseCard;
        switch (baseCard.CardType)
        {
            case CardType.Unit:
                InitUnitCard(baseCard as UnitCardStats);
                break;

            case CardType.Weapon:
                InitWeaponCard(baseCard as WeaponCardStats);
                break;

            case CardType.Spell:
                InitSpellCard(baseCard as SpellCardStats);
                break;
        }

        cardName.text = cardStats.CardName;
        manaText.text = cardStats.CurrentManaCost.ToString();
    }

    protected virtual void InitUnitCard(UnitCardStats unitCard)
    {
        if (unitCard == null) throw new ArgumentException("Invalid card type");
        unitComponents.SetActive(true);
        spellComponents.SetActive(false);
        weaponComponents.SetActive(false);
        unitAttackText.text = unitCard.CurrentAttack.ToString();
        unitHpText.text = unitCard.MaxHP.ToString();
        unitDescription.text = unitCard.Description;
    }

    protected virtual void InitWeaponCard(WeaponCardStats weaponCard)
    {
        if (weaponCard == null) throw new ArgumentException("Invalid card type");
        weaponComponents.SetActive(true);
        unitComponents.SetActive(false);
        spellComponents.SetActive(false);
        weaponAttackText.text = weaponCard.CurrentAttack.ToString();
        weaponDurabilityText.text = weaponCard.CurrentDurability.ToString();
        weaponDescription.text = weaponCard.Description;
    }

    protected virtual void InitSpellCard(SpellCardStats spellCard)
    {
        if (spellCard == null) throw new ArgumentException("Invalid card type");
        spellComponents.SetActive(true);
        weaponComponents.SetActive(false);
        unitComponents.SetActive(false);
        string formattedDescription = string.Format(spellCard.Description, spellCard.CurrentSpellDamage);
        spellDescription.text = formattedDescription;
    }
}