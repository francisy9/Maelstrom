using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class OnBoardDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Player player;
    private OnBoardCard onBoardCard;
    private OnBoardCard currentlyDetectedCard;

    public void InitDragCardController(Player player) {
        this.player = player;
        onBoardCard = GetComponent<OnBoardCard>();
        onBoardCard.OnDeath += Card_OnDeath;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!player.IsTurn()) {
            Debug.Log("Not your turn");
            eventData.pointerDrag = null;
            return;
        }


        if (!onBoardCard.CanAttack()) {
            eventData.pointerDrag = null;
            return;
        }

        Vector3 attackFromVec3 = new Vector3(transform.position.x, transform.position.y, -1);
        Vector3 attackToVec3 = new Vector3(eventData.position.x, eventData.position.y, -1);
        LineController.Instance.Show();
        LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);

    }

    public void OnDrag(PointerEventData eventData)
    {
        GameObject opponentCard = DetectCard(eventData);
        Vector3 attackFromVec3 = new Vector3(transform.position.x, transform.position.y, -1);
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

        if (opponentCard != null) {
            currentlyDetectedCard = opponentCard.GetComponent<OnBoardCard>();
            pointerWorldPos = opponentCard.transform.position;
        } else {
            currentlyDetectedCard = null;
        }

        Vector3 attackToVec3 = new Vector3(pointerWorldPos.x, pointerWorldPos.y, -1);
        LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentlyDetectedCard) {
            RequestAttack();
        }
        LineController.Instance.Hide();
    }

    private GameObject DetectCard(PointerEventData pointerEventData) {
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(pointerEventData.position);
        LayerMask opponentCardLayerMask = LayerMask.GetMask(OPPONENT_PLAYED_CARD_LAYER);
        RaycastHit2D hit = Physics2D.Raycast(pointerWorldPos, Vector2.zero, 1.0f, opponentCardLayerMask);
        if (hit.collider != null) {
            GameObject opponentCardGameObject = hit.collider.gameObject;
            return opponentCardGameObject;
        }
        return null;
    }

    private void Card_OnDeath(object sender, EventArgs e) {
        throw new NotImplementedException();
    }

    private void RequestAttack() {
        int boardIndex = Board.Instance.GetBoardIndex(onBoardCard.GetCardUid());
        int enemyCardBoardIndex = EnemyBoard.Instance.GetBoardIndex(currentlyDetectedCard.GetCardUid());
        player.RequestAttack(boardIndex, enemyCardBoardIndex);
    }
}
