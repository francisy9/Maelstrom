using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class Board : MonoBehaviour
{
    public void ResetCardAttacks() {
        foreach (Transform child in transform) {
            Card card = child.GetComponent<Card>();
            card.ResetAttack();
        }
            
    }
}
