using TMPro;
using UnityEngine;
using static Types;

public class InHandCardVisual : MonoBehaviour
{
    [SerializeField] private InHandCard card;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private GameObject cardVisualPlaceHolderObject;
    private CanvasGroup canvasGroup;
    private GameObject cardVisualPlaceHolder;
    private UnitCardStats cardStats;
    private bool enlarged;
    private bool beingDragged;
    

    public void InitVisual() {
        cardStats = card.GetCardStats();
        cardName.text = cardStats.CardName;
        description.text = cardStats.Description;
        manaText.text = cardStats.CurrentManaCost.ToString();
        attackText.text = cardStats.CurrentAttack.ToString();
        hpText.text = cardStats.MaxHP.ToString();
        enlarged = false;
        beingDragged = false;
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    public void ProjectCardOnHover() {
        if (enlarged || beingDragged) {
            return;
        }
        canvasGroup.alpha = 0f;
        cardVisualPlaceHolder = Instantiate(cardVisualPlaceHolderObject, HandController.Instance.transform);
        CardVisualPlaceHolder placeHolder = cardVisualPlaceHolder.GetComponent<CardVisualPlaceHolder>();
        placeHolder.InitVisual(cardStats.CardName, cardStats.Description, cardStats.CurrentManaCost, cardStats.CurrentAttack, cardStats.MaxHP);
        cardVisualPlaceHolder.transform.localScale *= 1.2f;
        Vector3 hoverPos = transform.position;
        hoverPos.y += 100;
        cardVisualPlaceHolder.transform.position = hoverPos;
        enlarged = true;
    }

    public void UnHoverCard() {
        if (enlarged) {
            Destroy(cardVisualPlaceHolder);
            cardVisualPlaceHolder.transform.SetParent(null);
            canvasGroup.alpha = 1f;
            enlarged = false;
        }
    }

    public void BeingDrag() {
        UnHoverCard();
        beingDragged = true;
    }

    public void EndDrag() {
        beingDragged = false;
    }
 }
