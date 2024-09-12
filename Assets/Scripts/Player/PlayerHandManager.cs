using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using CardTypes;

public class PlayerHandManager : NetworkBehaviour
{
    private Player player;
    [SerializeField] private HandController handController;
    [SerializeField] private GameObject cardObject;
    [SerializeField] private GameObject cardBackObject;
    [SerializeField] private Player enemy;
    [SerializeField] private EnemyHand enemyHand;
    public void Initialize(Player player) {
        this.player = player;
        handController.SetPlayer(player);
        enemyHand.SetPlayer(player.GetOpponent());
    }

    public void AddCardToHand(BaseCard baseCard) {
        handController.AddCardToHand(baseCard);
    }

    public void AddCardToOpponentHand() {
        enemyHand.AddCardToHand();
    }

    public void PlayCard(int handIndex, int boardIndex) {
        handController.PlayUnitCard(handIndex, boardIndex);
    }

    public void OpponentPlayCard(byte[] cardData, int handIndex, int boardIndex) {
        enemyHand.PlayCard(cardData, handIndex, boardIndex);
    }
}
