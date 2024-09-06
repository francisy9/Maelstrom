using UnityEngine;
using static Types;

public class BaseInHandCard : MonoBehaviour
{
    [SerializeField] private BaseInHandCardVisual cardVisual;
    private BaseCard cardStats;

    public void InitCard(BaseCard cardStats) {
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
}
