using UnityEngine;
using static Types;

public class InHandCard : MonoBehaviour
{
    [SerializeField] private InHandCardVisual cardVisual;
    private BaseCard cardStats;

    public void InitCard(BaseCard cardStats) {
        this.cardStats = cardStats;
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
}
