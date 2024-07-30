using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class DragCardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private DropZone playerDropZone;
    [SerializeField] private CardVisual cardVisual;
    private Player player;
    public event EventHandler OnCardPlayed;
    private CanvasGroup canvasGroup;
    private CardLocation cardLocation;
    private Transform prevParentTransform;
    private Card card;
    private Card currentlyDetectedCard;

    public void InitDragCardController(Player player) {
        canvas = player.GetCanvas();
        playerDropZone = player.GetDropZone();
        this.player = player;
        canvasGroup = GetComponent<CanvasGroup>();
        prevParentTransform = transform.parent;
        card = GetComponent<Card>();
        card.OnDeath += Card_OnDeath;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        switch (cardLocation) {
            case CardLocation.Hand:
                if (!CanPlayCard()) {
                    eventData.pointerDrag = null;
                    return;
                }
                canvasGroup.alpha = .6f;
                canvasGroup.blocksRaycasts = false;
                transform.SetParent(canvas.transform);
                break;
            case CardLocation.Board:
                Vector3 attackFromVec3 = new Vector3(transform.position.x, transform.position.y, -1);
                Vector3 attackToVec3 = new Vector3(eventData.position.x, eventData.position.y, -1);
                LineController.Instance.Show();
                LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);
                break;
            case CardLocation.Discard:
                Debug.LogError("Should not be able to drag discarded card");
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (cardLocation) {
            case CardLocation.Hand:
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out pos);
                transform.position = canvas.transform.TransformPoint(pos);
                break;
            case CardLocation.Board:
                GameObject opponentCard = DetectCard(eventData);
                Vector3 attackFromVec3 = new Vector3(transform.position.x, transform.position.y, -1);
                Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

                if (opponentCard != null) {
                    currentlyDetectedCard = opponentCard.GetComponent<Card>();
                    Debug.Log(opponentCard.name);
                    pointerWorldPos = opponentCard.transform.position;
                } else {
                    currentlyDetectedCard = null;
                }

                Vector3 attackToVec3 = new Vector3(pointerWorldPos.x, pointerWorldPos.y, -1);
                LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);
                break;
            case CardLocation.Discard:
                Debug.LogError("Should not be able to drag discarded card");
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        switch (cardLocation) {
            case CardLocation.Hand:
                if (DropZoneIsPointerOver(eventData)) {
                    Transform boardTransform = playerDropZone.transform;
                    transform.SetParent(boardTransform);
                    MoveCardToBoard(boardTransform);
                    player.ConsumeMana(card.GetManaCost());
                } else {
                    transform.SetParent(prevParentTransform);
                }
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                break;
            case CardLocation.Board:
                if (currentlyDetectedCard) {
                    gameObject.GetComponent<Card>().AttackCard(currentlyDetectedCard);
                }
                LineController.Instance.Hide();
                break;
            case CardLocation.Discard:
                Debug.LogError("Should not be able to drag discarded card");
                break;
        }
    }

    private bool CanPlayCard() {
        if (player.GetRemainingMana() < card.GetManaCost()) {
            Debug.Log($"{player.GetRemainingMana()} card mana: {card.GetManaCost()}");
            Debug.Log("Insufficient mana to play card");
            return false;
        }
        if (!player.IsTurn()) {
            Debug.Log("Not your turn");
            return false;
        }
        return true;
    }

    public void MoveCardToBoard(Transform boardTransform) {
        cardLocation = CardLocation.Board;
        prevParentTransform = boardTransform;
        OnCardPlayed.Invoke(this, EventArgs.Empty);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        gameObject.layer = LayerMask.NameToLayer(PLAYER_CARD_LAYER);
    }

    public CardLocation GetCardLocation() {
        return cardLocation;
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

    private bool DropZoneIsPointerOver(PointerEventData eventData) {
        RectTransform dropZoneRectTransform = playerDropZone.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(dropZoneRectTransform, eventData.position, eventData.pressEventCamera);
    }


    private void DiscardSelf() {
        cardLocation = CardLocation.Discard;
    }
}
