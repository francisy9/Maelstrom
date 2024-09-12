using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CardTypes;
using static Layers.Layers;
using static Const.Const;

public class DragCardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InHandCardVisual cardVisual;
    private Canvas canvas;
    private Board playerDropZone;
    private Player player;
    public event EventHandler OnCardPlayed;
    private CanvasGroup canvasGroup;
    private Transform prevParentTransform;
    private HandController handController;
    private InHandCard card;
    private int uid;
    private int proposedBoardIndex;
    private Vector3 collapsedPos;
    private float collapsedRotation;
    private Vector3 expandedPos;
    private float expandedRotation;
    private bool canBeDragged;
    private CardType cardType;
    private GameObject currentlyDetectedTarget;
    private bool needsTargeting;
    private Targetable targetable;
    private LayerMask targetLayerMask;

    public void InitDragCardController(Player player, HandController handController, int uid) {
        canvas = player.GetCanvas();
        playerDropZone = player.GetBoard();
        this.player = player;
        canvasGroup = GetComponent<CanvasGroup>();
        prevParentTransform = transform.parent;
        this.handController = handController;
        card = GetComponent<InHandCard>();
        cardType = card.GetCardType();
        this.uid = uid;
        canBeDragged = false;
        needsTargeting = NeedsTargeting(card.GetCardStats());
        if (needsTargeting) {
            targetable = GetTargetable(card.GetCardStats());
            targetLayerMask = GetLayerMaskByTargetable(targetable);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (HandVisual.Instance.IsExpanded()) {
            cardVisual.ProjectCardOnHover();
            canBeDragged = true;
        }
    }

    // TODO: Handle edge case where card was being hovered when another card was drawn
    public void OnPointerExit(PointerEventData eventData) {
        cardVisual.UnHoverCard();
        canBeDragged = false;
    }

    public void SetCollapsedPos(Vector3 pos, float angle) {
        collapsedPos = pos;
        collapsedRotation = angle;
    }

    public void SetExpandedPos(Vector3 pos, float angle) {
        expandedPos = pos;
        expandedRotation = angle;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canBeDragged) {
            eventData.pointerDrag = null;
            return;
        }

        if (!player.IsTurn()) {
            Debug.Log("Not your turn");
            eventData.pointerDrag = null;
            return;
        }

        if (player.GetMana() < card.GetCardStats().CurrentManaCost) {
            Debug.Log("Insufficient mana");
            eventData.pointerDrag = null;
            return;
        }

        if (handController.IsHandControllerBusy()) {
            Debug.Log("Hand Controller busy");
            eventData.pointerDrag = null;
            return;
        }

        cardVisual.BeingDrag();

        HandController.Instance.UnblockRayCasts();

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        handController.LocalTryingToPlayCard(uid);
        playerDropZone.UpdateXPos();
        transform.localEulerAngles = Vector3.zero;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);

        switch (cardType) {
            case CardType.Unit:
                proposedBoardIndex = playerDropZone.GetProposedBoardIndex(eventData);
                break;
            case CardType.Spell:
                SpellCardStats spellStats;
                if (card.TryGetSpellStats(out spellStats)) {
                    if (LeftHandArea(eventData)) {
                        if (needsTargeting) {
                            currentlyDetectedTarget = DetectTarget(eventData, spellStats);

                            Vector3 attackFromVec3 = new Vector3(Hero.Instance.transform.position.x, Hero.Instance.transform.position.y, -1);
                            Vector3 attackToVec3;
                            LineController.Instance.Show();
                            if (currentlyDetectedTarget) {
                                attackToVec3 = currentlyDetectedTarget.transform.position;
                            } else {
                                attackToVec3 = Camera.main.ScreenToWorldPoint(eventData.position);
                            }
                            LineController.Instance.SetAttackLine(attackFromVec3, attackToVec3);
                        }
                    }
                }
                break;
            case CardType.Weapon:
                break;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        switch (cardType) {
            case CardType.Unit:
                if (DropZoneIsPointerOver(eventData) && proposedBoardIndex != -1) {
                    handController.RequestPlayCard(uid, proposedBoardIndex);
                } else {
                    handController.ReturnCardToHand();
                    playerDropZone.DestroyPlaceHolder();
                    proposedBoardIndex = -1;
                }
                break;
            case CardType.Spell:
                if (LeftHandArea(eventData)) {
                    if (needsTargeting) {
                        if (currentlyDetectedTarget) {
                            Targeting targeting = GetTargeting(currentlyDetectedTarget);
                            handController.RequestCastSpell(uid, targeting);
                        } else {
                            handController.ReturnCardToHand();
                        }
                    } else {
                        handController.RequestCastSpell(uid, null);
                    }
                } else {
                    handController.ReturnCardToHand();
                }
                break;
            case CardType.Weapon:
                break;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        HandController.Instance.BlockRayCasts();
    }

    private bool DropZoneIsPointerOver(PointerEventData eventData) {
        RectTransform dropZoneRectTransform = playerDropZone.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(dropZoneRectTransform, eventData.position, eventData.pressEventCamera);
    }

    private bool LeftHandArea(PointerEventData eventData) {
        RectTransform handRectTransform = HandVisual.Instance.GetComponent<RectTransform>();
        return !RectTransformUtility.RectangleContainsScreenPoint(handRectTransform, eventData.position, eventData.pressEventCamera);
    }

    public void ReturnCardToHand(int previousHandIndex) {
        transform.SetParent(prevParentTransform);
        transform.SetSiblingIndex(previousHandIndex);
        if (HandVisual.Instance.IsExpanded()) {
            transform.SetLocalPositionAndRotation(expandedPos, Quaternion.Euler(0, 0, expandedRotation));
        } else {
            transform.SetLocalPositionAndRotation(collapsedPos, Quaternion.Euler(0, 0, collapsedRotation));
        }
        cardVisual.EndDrag();
    }

    private bool NeedsTargeting(BaseCard baseCard) {
        foreach (var effect in baseCard.cardEffects) {
            if (effect.needsTargeting) {
                return true;
            }
        }
        return false;
    }

    private GameObject DetectTarget(PointerEventData eventData, BaseCard baseCard) {
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);

        RaycastHit2D hit = Physics2D.Raycast(pointerWorldPos, Vector2.zero, 1.0f, targetLayerMask);
        if (hit.collider != null) {
            GameObject targetGameObject = hit.collider.gameObject;
            return targetGameObject;
        }
        return null;
    }

    private Targeting GetTargeting(GameObject targetGameObject)
    {
        switch (targetable)
        {
            case Targetable.AllEnemies:
            case Targetable.EnemyUnits:
                return GetEnemyTargeting(targetGameObject);

            case Targetable.AllAllies:
            case Targetable.AllyUnits:
                return GetAllyTargeting(targetGameObject);

            case Targetable.All:
                if (IsPlayerOrPlayerHeroLayer(targetGameObject))
                {
                    return GetAllyTargeting(targetGameObject);
                }
                else if (IsOpponentOrOpponentHeroLayer(targetGameObject))
                {
                    return GetEnemyTargeting(targetGameObject);
                }
                break;

            default:
                Debug.LogError("Invalid targetable");
                break;
        }
        return null;
    }

    private Targeting GetEnemyTargeting(GameObject targetGameObject)
    {
        if (targetGameObject.TryGetComponent(out OnBoardCard enemyCard))
        {
            return new Targeting
            {
                targetBoardIndex = player.GetEnemyBoard().GetBoardIndex(enemyCard.GetCardUid()),
                targetType = TargetType.Enemy,
            };
        }
        else if (targetGameObject.TryGetComponent(out EnemyHero _))
        {
            return new Targeting
            {
                targetBoardIndex = HERO_BOARD_INDEX,
                targetType = TargetType.Enemy,
            };
        }
        return null;
    }

    private Targeting GetAllyTargeting(GameObject targetGameObject)
    {
        if (targetGameObject.TryGetComponent(out OnBoardCard allyCard))
        {
            return new Targeting
            {
                targetBoardIndex = player.GetBoard().GetBoardIndex(allyCard.GetCardUid()),
                targetType = TargetType.Ally,
            };
        }
        else if (targetGameObject.TryGetComponent(out Hero _))
        {
            return new Targeting
            {
                targetBoardIndex = HERO_BOARD_INDEX,
                targetType = TargetType.Ally,
            };
        }
        return null;
    }

    private bool IsPlayerOrPlayerHeroLayer(GameObject targetGameObject)
    {
        return targetGameObject.layer == LayerMask.NameToLayer(PLAYER_PLAYED_CARD_LAYER) ||
               targetGameObject.layer == LayerMask.NameToLayer(HERO_LAYER);
    }

    private bool IsOpponentOrOpponentHeroLayer(GameObject targetGameObject)
    {
        return targetGameObject.layer == LayerMask.NameToLayer(OPPONENT_PLAYED_CARD_LAYER) ||
               targetGameObject.layer == LayerMask.NameToLayer(OPPONENT_HERO_LAYER);
    }

    private Targetable GetTargetable(BaseCard baseCard) {
        foreach (var effect in baseCard.cardEffects) {
            if (effect.needsTargeting) {
                return effect.targetable;
            }
        }
        return Targetable.None;
    }
}
