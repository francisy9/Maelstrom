using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using static Types;

public class Player : NetworkBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Player opponent;
    [SerializeField] private Hero hero;
    [SerializeField] private EnemyHero enemyHero;
    [SerializeField] private Board board;
    [SerializeField] private EnemyBoard enemyBoard;
    [SerializeField] private EnemyHand enemyHand;
    [SerializeField] private PlayerManaDisplay manaDisplay;
    [SerializeField] private PlayerManaDisplay enemyManaDisplay;
    [SerializeField] private List<UnitCardStatsSO> deck;
    [SerializeField] private GameObject cardObject;
    [SerializeField] private HandController handController;
    [SerializeField] private GameObject cardBackObject;
    [SerializeField] private Button endTurnButton;
    [SyncVar(hook = nameof(OnManaChange))]
    private int mana = 0;
    [SyncVar(hook = nameof(OnManaChange))]
    private int maxMana = 0;
    public event EventHandler OnStartTurn;
    [SyncVar] private bool inTurn;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        endTurnButton.onClick.AddListener(() =>
        {
            RequestEndTurn();
        });
        TargetInitializeManaDisplay();
        handController.SetPlayer(this);
        enemyHand.SetPlayer(opponent);
        board.SetPlayer(this);
    }

    [TargetRpc]
    public void TargetInitializeManaDisplay()
    {
        manaDisplay.InitializeManaDisplay(this);
        enemyManaDisplay.InitializeManaDisplay(opponent);
    }

    [TargetRpc]
    public void TargetBeginGame(bool first, int heroStartHp, int enemyHeroStartHp)
    {
        hero.InitHero(heroStartHp);
        enemyHero.InitHero(enemyHeroStartHp);
    }

    public Board GetDropZone()
    {
        return board;
    }

    [TargetRpc]
    public void TargetStartTurn()
    {
        inTurn = true;
        OnStartTurn?.Invoke(this, EventArgs.Empty);
    }

    private void OnManaChange(int _, int _a)
    {
        manaDisplay.UpdateManaVisual();
        enemyManaDisplay.UpdateManaVisual();
    }

    [TargetRpc]
    public void TargetStartOpponentTurn()
    {
    }

    [Server]
    public void IncrementMaxMana()
    {
        maxMana += 1;
    }

    [Server]
    public void ConsumeMana(int mana)
    {
        this.mana -= mana;
    }

    [Server]
    public void RefreshMana()
    {
        mana = maxMana;
    }

    [TargetRpc]
    public void AddCardToHand(byte[] serializedCardArray)
    {
        BaseCard baseCard = BaseCard.Deserialize(serializedCardArray);
        handController.AddCardToHand(baseCard);
    }

    [TargetRpc]
    public void AddCardToOpponentHand()
    {
        enemyHand.AddCardToHand();
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public int GetMana()
    {
        return mana;
    }

    public int GetTotalMana()
    {
        return maxMana;
    }

    public bool IsTurn()
    {
        return inTurn;
    }

    private void ResetCardAttacks()
    {
        board.ResetCardAttacks();
    }

    private void RequestEndTurn()
    {
        GameManager.Instance.CmdEndTurn();
    }

    [TargetRpc]
    public void TargetEndTurn()
    {
        ResetCardAttacks();
        inTurn = false;
    }

    [TargetRpc]
    public void TargetPlayCard(int handIndex, int boardIndex)
    {
        handController.PlayUnitCard(handIndex, boardIndex);
    }

    [TargetRpc]
    public void TargetOpponentPlayCard(byte[] cardData, int handIndex, int boardIndex)
    {
        enemyHand.PlayCard(cardData, handIndex, boardIndex);
    }

    [TargetRpc]
    public void TargetAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData)
    {
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        object attackerStats;
        object targetStats;

        if (attackerIsCard)
        {
            attackerStats = UnitCardStats.Deserialize(cardData);
        }
        else
        {
            attackerStats = HeroStats.Deserialize(cardData);
        }

        if (targetIsCard)
        {
            targetStats = UnitCardStats.Deserialize(opponentCardData);
        }
        else
        {
            targetStats = HeroStats.Deserialize(opponentCardData);
        }

        board.CardAttack(boardIndex, opponentBoardIndex, attackerStats, targetStats);
    }

    [TargetRpc]
    public void TargetOpponentAttackResponse(int boardIndex, int opponentBoardIndex, byte[] cardData, byte[] opponentCardData)
    {
        // Note the flip in boardIndex and opponentBoardIndex
        // attacker is the opponent
        // self unit is target
        bool targetIsCard = GameManager.Instance.IsCardAtBoardIndex(boardIndex);
        bool attackerIsCard = GameManager.Instance.IsCardAtBoardIndex(opponentBoardIndex);

        object targetStats;
        object attackerStats;

        if (targetIsCard)
        {
            targetStats = UnitCardStats.Deserialize(cardData);
        }
        else
        {
            targetStats = HeroStats.Deserialize(cardData);
        }

        if (attackerIsCard)
        {
            attackerStats = UnitCardStats.Deserialize(opponentCardData);
        }
        else
        {
            attackerStats = HeroStats.Deserialize(opponentCardData);
        }

        board.CardAttackedBy(boardIndex, opponentBoardIndex, targetStats, attackerStats);
    }

    public BoardBase GetBoard()
    {
        return board;
    }

    public BoardBase GetEnemyBoard()
    {
        return enemyBoard;
    }

    internal void RequestPlayCard(int handIndex, int boardIndex)
    {
        GameManager.Instance.CmdPlayUnitCard(handIndex, boardIndex);
        return;
    }

    internal void RequestAttack(int boardIndex, int opponentBoardIndex)
    {
        GameManager.Instance.CmdAttack(boardIndex, opponentBoardIndex);
        return;
    }



    // Functions for running functions on server
    [Server]
    public void ServerAddCardToHand(UnitCardStatsSO cardStatsSO)
    {
        UnitCardStats cardStats = new UnitCardStats(cardStatsSO);
        handController.AddCardToHand(cardStats);
    }

    [Server]
    public void ServerAddCardToOpponentHand()
    {
        enemyHand.AddCardToHand();
    }

    [Server]
    public void SetPlayerInTurn()
    {
        inTurn = true;
    }
}
