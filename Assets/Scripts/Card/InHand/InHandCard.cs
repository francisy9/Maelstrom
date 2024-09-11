using UnityEngine;
using CardTypes;

public class InHandCard : MonoBehaviour
{
    [SerializeField] private InHandCardVisual cardVisual;
    private BaseCard cardStats;

    public void InitCard(BaseCard cardStats) {
        Debug.Log($"Init card card stats: {cardStats.CardName} {cardStats.CardType}");
        this.cardStats = cardStats;
        if (cardStats == null) {
            Debug.LogError("Init card card stats is null");
        }
        cardVisual.InitVisual();
    }

    public void DestroySelf() {
        transform.SetParent(null);
        Destroy(gameObject);
    }

    public BaseCard GetCardStats() {
        return cardStats;
    }

    public CardType GetCardType() {
        return cardStats.CardType;
    }

    public bool TryGetSpellStats(out SpellCardStats spellStats)
    {
        if (cardStats.CardType == CardType.Spell)
        {
            spellStats = (SpellCardStats)cardStats;
            return true;
        }
        Debug.Log("No spell stats");
        spellStats = null;
        return false;
    }
}
