using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Types;

public class DragCardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CardVisual cardVisual;
    public event EventHandler OnCardPlayed;
    private CanvasGroup canvasGroup;
    public static Transform zone;
    private CardLocation cardLocation;
    private Transform prevParentTransform;
    private Card currentlyDetectedCard;
    
    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start() {
        prevParentTransform = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("On begin drag");
        switch (cardLocation) {
            case CardLocation.Hand:
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
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                transform.SetParent(prevParentTransform);
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
            Debug.Log("Game object found");
            GameObject opponentCardGameObject = hit.collider.gameObject;
            return opponentCardGameObject;
        }
        Debug.Log("no object found");
        return null;
    }
}
