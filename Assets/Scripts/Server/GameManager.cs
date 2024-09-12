using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using CardTypes;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player playerOne;
    // TODO: @max figure out how to load this in on run time based on user
    [SerializeField] private List<BaseCardSO> p1Deck;
    [SerializeField] private Sprite p1HeroSprite;
    [SerializeField] private int p1HeroMaxHp;
    private bool p1Assigned = false;
    [SerializeField] private Player playerTwo;
    [SerializeField] private List<BaseCardSO> p2Deck;
    [SerializeField] private Sprite p2HeroSprite;
    [SerializeField] private int p2HeroMaxHp;
    private bool p2Assigned = false;
    [SerializeField] private Button endTurnButton;
    public event EventHandler OnGameStart;

    [SyncVar] private int turn;
    [SyncVar] private bool isP1Turn;

    public override void OnStartServer() {
        Instance = this;
        isP1Turn = UnityEngine.Random.Range(0, 2) == 0;

        // TODO: Testing logic, remove when done
        isP1Turn = true;


        turn = 0;
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isServer) return;

        Instance = this;
    }

    // Used for initialization of game by player manager
    public Player GetPlayer() {
        if (!p1Assigned) {
            p1Assigned = true;
            return playerOne;
        } else if (!p2Assigned) {
            p2Assigned = true;
            return playerTwo;
        } else {
            Debug.LogError("Too many attempted connections");
            return null;
        }
    }

    public void StartGame() {
        Debug.Log("start game");
        ServerState.Instance.SetPlayerRefs(playerOne, playerTwo);

        Player starter = GetInTurnPlayer();
        Player nextPlayer = GetNextPlayer();
        int numCardsToBeDrawnByFirstPlayer = 3;
        int numCardsToBeDrawnBySecondPlayer = 4;

        // int numCardsToBeDrawnByFirstPlayer = 1;
        // int numCardsToBeDrawnBySecondPlayer = 0;

        for (int i = 0; i < numCardsToBeDrawnByFirstPlayer; i++) {
            BaseCardSO cardDrawn = DrawBaseCardSO(starter);
            CardType cardType = cardDrawn.GetCardType();
            BaseCard baseCard = ServerState.Instance.GetBaseCardFromSO(cardDrawn);
            ServerState.Instance.AddCardToHand(baseCard, starter);


            switch (cardType)
            {
                case CardType.Unit:
                    byte[] serializedUnitCardData = (baseCard as UnitCardStats).Serialize();
                    starter.TargetAddCardToHand(serializedUnitCardData);
                    break;
                case CardType.Spell:
                    byte[] serializedSpellCardData = (baseCard as SpellCardStats).Serialize();
                    starter.TargetAddCardToHand(serializedSpellCardData);
                    break;
                default:
                    Debug.LogError("Card type not implemented");
                    break;
            }
            nextPlayer.TargetAddCardToOpponentHand();
        }

        for (int j = 0; j < numCardsToBeDrawnBySecondPlayer; j++) {
            BaseCardSO cardDrawn = DrawBaseCardSO(nextPlayer);
            CardType cardType = cardDrawn.GetCardType();
            BaseCard baseCard = ServerState.Instance.GetBaseCardFromSO(cardDrawn);
            ServerState.Instance.AddCardToHand(baseCard, nextPlayer);

            switch (cardType)
            {
                case CardType.Unit:
                    byte[] serializedUnitCardData = (baseCard as UnitCardStats).Serialize();
                    nextPlayer.TargetAddCardToHand(serializedUnitCardData);
                    break;
                default:
                    Debug.LogError("Card type not implemented");
                    break;
            }

            starter.TargetAddCardToOpponentHand();
        }
        InitializeManaDisplays();
        starter.IncrementMaxMana();


        // TODO: testing, remove when done
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        starter.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();
        nextPlayer.IncrementMaxMana();



        starter.RefreshMana();

        int starterHeroHp = isP1Turn ? p1HeroMaxHp : p2HeroMaxHp;
        int nextPlayerHeroHp = isP1Turn ? p2HeroMaxHp : p1HeroMaxHp;
        
        ServerState.Instance.InitHeroes(p1HeroMaxHp, p2HeroMaxHp);
        starter.TargetBeginGame(true, starterHeroHp, nextPlayerHeroHp);
        nextPlayer.TargetBeginGame(false, nextPlayerHeroHp, starterHeroHp);

        OnGameStart?.Invoke(this, EventArgs.Empty);
        StartTurn();
        ServerState.Instance.PrintServerGameState();
    }

    private void InitializeManaDisplays() {
        playerOne.TargetInitializeManaDisplay();
        playerTwo.TargetInitializeManaDisplay();
    }

    private void StartTurn() {
        GetInTurnPlayer().TargetStartTurn();
        GetNextPlayer().TargetStartOpponentTurn();
    }

    public bool IsP1Turn() {
        return isP1Turn;
    }

    public Player GetInTurnPlayer() {
        return isP1Turn ? playerOne : playerTwo;
    }

    public Player GetNextPlayer() {
        return isP1Turn ? playerTwo : playerOne;
    }

    private bool IsPlayerInTurn(Player requestingPlayer) {
        return GetInTurnPlayer() == requestingPlayer;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn(NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        
        if (IsPlayerInTurn(requestingPlayer)) {
            ServerState.Instance.EndTurn(requestingPlayer);
            requestingPlayer.TargetEndTurn();
            isP1Turn = !isP1Turn;
            Player playerNextInTurn = GetInTurnPlayer();
            playerNextInTurn.IncrementMaxMana();
            playerNextInTurn.RefreshMana();
            playerNextInTurn.TargetStartTurn();
            turn += 1;
        } else {
            Debug.LogError("Wrong player was able to request end turn");
        }
    }

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

    [Server]
    private BaseCardSO DrawBaseCardSO(Player player) {
        List<BaseCardSO> deckToDrawFrom = player == playerOne ? p1Deck : p2Deck;
        int cardIndex = UnityEngine.Random.Range(0, deckToDrawFrom.Count);
        BaseCardSO cardDrawn = deckToDrawFrom[cardIndex];
        deckToDrawFrom.RemoveAt(cardIndex);
        return cardDrawn;
    }

    // Functions to test on server client
    public void TestingCards() {
        isP1Turn = true;
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.RefreshMana();
        HandController.Instance.SetPlayer(playerOne);
    }

    // Server functions for debugging purposes
    public int GetP1Mana() {
        return playerOne.GetMana();
    }

    public int GetP2Mana() {
        return playerTwo.GetMana();
    }

    public int GetP1MaxMana() {
        return playerOne.GetMaxMana();
    }

    public int GetP2MaxMana() {
        return playerTwo.GetMaxMana();
    }

    public Player GetPlayerOne() {
        return playerOne;
    }

    public Player GetPlayerTwo() {
        return playerTwo;
    }
    // End of server debug functions
}
