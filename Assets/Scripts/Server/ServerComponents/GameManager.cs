using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using CardTypes;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int p1HeroMaxHp;
    [SerializeField] private int p2HeroMaxHp;
    public event EventHandler OnGameStart;
    [SerializeField] private ServerPlayerManager playerManager;
    [SerializeField] private ServerHandManager handManager;
    [SerializeField] private ServerTurnManager turnManager;
    [SerializeField] private ServerManaManager manaManager;
    [SerializeField] private ServerNetworkingManager networkingManager;
    public override void OnStartServer() {
        Instance = this;
        base.OnStartServer();
        InitializeComponents();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isServer) return;
        Instance = this;
    }

    private void InitializeComponents() {
        turnManager.Initialize();
    }

    // Used for initialization of game by player manager
    public Player GetPlayerRefBeforeStart() => playerManager.GetPlayerRefBeforeStart();

    public void StartGame() {
        Debug.Log("start game");
        ServerState.Instance.SetPlayerRefs(playerManager.GetPlayerOne(), playerManager.GetPlayerTwo());

        Player starter = GetInTurnPlayer();
        Player nextPlayer = GetNextPlayer();
        handManager.StartGameDrawCards();

        GetManaManager().InitializeManaDisplays();
        starter.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        starter.RefreshMana();

        int starterHeroHp = turnManager.IsP1Turn() ? p1HeroMaxHp : p2HeroMaxHp;
        int nextPlayerHeroHp = turnManager.IsP1Turn() ? p2HeroMaxHp : p1HeroMaxHp;
        
        ServerState.Instance.InitHeroes(p1HeroMaxHp, p2HeroMaxHp);
        starter.TargetBeginGame(true, starterHeroHp, nextPlayerHeroHp);
        nextPlayer.TargetBeginGame(false, nextPlayerHeroHp, starterHeroHp);

        OnGameStart?.Invoke(this, EventArgs.Empty);
        StartTurn();
        ServerState.Instance.PrintServerGameState();
    }

    private void StartTurn() => turnManager.StartTurn();
    public bool IsP1Turn() => turnManager.IsP1Turn();

    private bool IsPlayerInTurn(Player requestingPlayer) => turnManager.IsPlayerInTurn(requestingPlayer);

    public void CmdEndTurn(NetworkConnectionToClient sender = null) => networkingManager.CmdEndTurn(sender);

    [Command(requiresAuthority = false)]
    public void CmdPlayUnitCard(int handIndex, int boardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        

        if (handIndex < 0 || handIndex > 9 || boardIndex < 0 || boardIndex > 6) {
            Debug.LogError($"{requestingPlayer.name} supplied argument handIndex: {handIndex} boardIndex: {boardIndex}");
        }

        if(IsPlayerInTurn(requestingPlayer)) {
            BaseCard baseCard = ServerState.Instance.GetCardStatsAtHandIndex(handIndex, requestingPlayer);

            if (baseCard.GetCardType() != CardType.Unit) {
                Debug.LogError($"Card type mismatch: {requestingPlayer.name} is trying to play unit, but card has type: {baseCard.GetCardType()}");
            }

            UnitCardStats cardToBePlayed = ServerState.Instance.GetCardStatsAtHandIndex(handIndex, requestingPlayer) as UnitCardStats;

            if (requestingPlayer.GetMana() < cardToBePlayed.CurrentManaCost) {
                Debug.LogError("Insufficient mana to play card but still able to make request");
                return;
            }

            ServerState.Instance.MoveUnitCardToBoard(requestingPlayer, handIndex, cardToBePlayed, boardIndex);

            requestingPlayer.ConsumeMana(cardToBePlayed.CurrentManaCost);
            requestingPlayer.TargetPlayCard(handIndex, boardIndex);

            byte[] cardData = cardToBePlayed.Serialize();
            GetNextPlayer().TargetOpponentPlayCard(cardData, handIndex, boardIndex);


            ServerState.Instance.PrintServerGameState();
        } else {
            Debug.LogError("Player isn't in turn");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdAttack(int boardIndex, int opponentBoardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        Debug.Log($"{requestingPlayer.name} is requesting to attack card own board index: {boardIndex} opponent board index: {opponentBoardIndex}");
        
        if(IsPlayerInTurn(requestingPlayer)) {
            bool attackerIsCard;
            if (IsCardAtBoardIndex(boardIndex)) {
                // Attacker is card type
                attackerIsCard = true;
                UnitCardStats card = ServerState.Instance.GetCardStatsAtBoardIndex(boardIndex, requestingPlayer);

                if (card.CurrentAttack <= 0) {
                    Debug.LogError("Unit's attack value is 0!");
                    return;
                }

                if (card.NumAttacks <= 0) {
                    Debug.LogError("Insufficient attacks remaining");
                }
            } else {
                // Attacker is hero
                attackerIsCard = false;
                HeroStats hero = ServerState.Instance.GetHeroStats(requestingPlayer);

                if (hero.CurrentAttack <= 0) {
                    Debug.LogError("Hero's attack value is 0!");
                    return;
                }

                if (hero.NumAttacks <= 0) {
                    Debug.LogError("Insufficient attacks remaining");
                }
            }

            byte[] attackerData;
            byte[] targetData;
            object[] serverUpdateResponses = ServerState.Instance.Attack(boardIndex, opponentBoardIndex, requestingPlayer);

            if (attackerIsCard) {
                attackerData = (serverUpdateResponses[0] as UnitCardStats).Serialize();
            } else {
                attackerData = (serverUpdateResponses[0] as HeroStats).Serialize();
            }

            if (IsCardAtBoardIndex(opponentBoardIndex)) {
                targetData = (serverUpdateResponses[1] as UnitCardStats).Serialize();
            } else {
                targetData = (serverUpdateResponses[1] as HeroStats).Serialize();
            }

            requestingPlayer.TargetAttackResponse(boardIndex, opponentBoardIndex, attackerData, targetData);
            GetNextPlayer().TargetOpponentAttackResponse(opponentBoardIndex, boardIndex, targetData, attackerData);

            // ServerUpdate.Instance.PrintServerGameState();
        } else {
            Debug.LogError("Player isn't in turn");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdCastSpell(int handIndex, Targeting targeting, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        if (targeting == null) {
            Debug.Log($"{requestingPlayer.name} is requesting to cast spell at hand index: {handIndex} targeting null");
        } else {
            Debug.Log($"{requestingPlayer.name} is requesting to cast spell at hand index: {handIndex} targeting {targeting.targetType} index: {targeting.targetBoardIndex}");
        }

        if (IsPlayerInTurn(requestingPlayer)) {
            SpellCardStats cardToBeCast = ServerState.Instance.GetCardStatsAtHandIndex(handIndex, requestingPlayer) as SpellCardStats;
            ServerState.Instance.CastSpell(handIndex, targeting, requestingPlayer);
        } else {
            Debug.LogError("Player isn't in turn");
        }
    }   

    public bool IsCardAtBoardIndex(int boardIndex) {
        if (boardIndex >= 0 & boardIndex <= 6) {
            return true;
        }
        return false;
    }

    // Functions to test on server client
    public void TestingCards() {
    //     isP1Turn = true;
    //     GetInTurnPlayer().IncrementMaxMana();
    //     GetInTurnPlayer().RefreshMana();
    //     HandController.Instance.SetPlayer(GetInTurnPlayer());
    }

    public Player GetPlayerOne() => playerManager.GetPlayerOne();
    public Player GetPlayerTwo() => playerManager.GetPlayerTwo();
    public Player GetInTurnPlayer() => playerManager.GetInTurnPlayer();
    public Player GetNextPlayer() => playerManager.GetNextPlayer();
    public Player GetOpposingPlayer(Player player) => playerManager.GetOpposingPlayer(player);
    public ServerTurnManager GetTurnManager() => turnManager;
    public ServerPlayerManager GetPlayerManager() => playerManager;
    public ServerManaManager GetManaManager() => manaManager;
}
