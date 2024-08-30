using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using static Types;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player playerOne;
    // TODO: @max figure out how to load this in on run time based on user
    [SerializeField] private List<CardStatsSO> p1Deck;
    [SerializeField] private Sprite p1HeroSprite;
    [SerializeField] private int p1HeroMaxHp;
    private bool p1Assigned = false;
    [SerializeField] private Player playerTwo;
    [SerializeField] private List<CardStatsSO> p2Deck;
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
        ServerUpdate.Instance.SetPlayerRefs(playerOne, playerTwo);

        Player starter = GetInTurnPlayer();
        Player nextPlayer = GetNextPlayer();
        int numCardsToBeDrawnByFirstPlayer = 3;
        int numCardsToBeDrawnBySecondPlayer = 4;

        for (int i = 0; i < numCardsToBeDrawnByFirstPlayer; i++) {
            CardStatsSO cardDrawn = DrawCardStatSO(starter);
            starter.AddCardToHand(cardDrawn);
            nextPlayer.AddCardToOpponentHand();
            ServerUpdate.Instance.AddCardToHand(cardDrawn, starter);
        }

        for (int j = 0; j < numCardsToBeDrawnBySecondPlayer; j++) {
            CardStatsSO cardDrawn = DrawCardStatSO(nextPlayer);
            nextPlayer.AddCardToHand(cardDrawn);
            starter.AddCardToOpponentHand();
            ServerUpdate.Instance.AddCardToHand(cardDrawn, nextPlayer);
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
        // Sprite starterHeroSprite = isP1Turn ? p1HeroSprite : p2HeroSprite;
        // Sprite nextPlayerHeroSprite = isP1Turn ? p2HeroSprite : p1HeroSprite;

        // starter.TargetStartGame(true, starterHeroHp, starterHeroSprite, nextPlayerHeroHp, nextPlayerHeroSprite);
        // nextPlayer.TargetStartGame(false, nextPlayerHeroHp, nextPlayerHeroSprite, starterHeroHp, starterHeroSprite);

        starter.TargetStartGame(true, starterHeroHp, nextPlayerHeroHp);
        nextPlayer.TargetStartGame(false, nextPlayerHeroHp, starterHeroHp);

        OnGameStart?.Invoke(this, EventArgs.Empty);
        StartTurn();
        ServerUpdate.Instance.PrintServerGameState();
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
        Debug.Log($"end turn called, sender: {sender.identity}");
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        
        if (IsPlayerInTurn(requestingPlayer)) {
            ServerUpdate.Instance.EndTurn(requestingPlayer);
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
    public void CmdPlayCard(int handIndex, int boardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        

        if (handIndex < 0 || handIndex > 9 || boardIndex < 0 || boardIndex > 6) {
            Debug.LogError($"{requestingPlayer.name} supplied argument handIndex: {handIndex} boardIndex: {boardIndex}");
        }

        if(IsPlayerInTurn(requestingPlayer)) {
            CardStats cardToBePlayed = ServerUpdate.Instance.GetCardStatsAtHandIndex(handIndex, requestingPlayer);

            if (requestingPlayer.GetMana() < cardToBePlayed.CurrentManaCost) {
                Debug.LogError("Insufficient mana to play card but still able to make request");
                return;
            }

            ServerUpdate.Instance.MoveCardToBoard(requestingPlayer, handIndex, cardToBePlayed, boardIndex);

            requestingPlayer.ConsumeMana(cardToBePlayed.CurrentManaCost);
            requestingPlayer.TargetPlayCard(boardIndex);

            byte[] cardData = cardToBePlayed.Serialize();
            GetNextPlayer().TargetOpponentPlayCard(cardData, handIndex, boardIndex);


            ServerUpdate.Instance.PrintServerGameState();
        } else {
            Debug.LogError("Player isn't in turn");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdAttack(int boardIndex, int opponentBoardIndex, NetworkConnectionToClient sender = null) {
        Player requestingPlayer = sender.identity.GetComponent<Player>();
        Debug.Log($"{requestingPlayer.name} is requesting to attack card own board index: {boardIndex} opponent board index: {opponentBoardIndex}");
        
        if(IsPlayerInTurn(requestingPlayer)) {
            CardStats card = ServerUpdate.Instance.GetCardStatsAtBoardIndex(boardIndex, requestingPlayer);

            if (card.CurrentAttack <= 0) {
                Debug.LogError("Unit's attack value is 0!");
                return;
            }

            if (card.NumAttacks <= 0) {
                Debug.LogError("Insufficient attacks remaining");
            }

            CardStats[] cardsPostAttack = ServerUpdate.Instance.Attack(boardIndex, opponentBoardIndex, requestingPlayer);

            byte[] cardData = cardsPostAttack[0].Serialize();
            byte[] opponentCardData = cardsPostAttack[1].Serialize();
            requestingPlayer.TargetAttackCard(boardIndex, opponentBoardIndex, cardData, opponentCardData);
            GetNextPlayer().TargetOpponentAttackCard(opponentBoardIndex, boardIndex, opponentCardData, cardData);

            ServerUpdate.Instance.PrintServerGameState();
        } else {
            Debug.LogError("Player isn't in turn");
        }
    }

    [Server]
    private CardStatsSO DrawCardStatSO(Player player) {
        List<CardStatsSO> deckToDrawFrom = player == playerOne ? p1Deck : p2Deck;
        int cardIndex = UnityEngine.Random.Range(0, deckToDrawFrom.Count);
        CardStatsSO cardDrawn = deckToDrawFrom[cardIndex];
        deckToDrawFrom.RemoveAt(cardIndex);
        return cardDrawn;
    }

    // Functions to test on server client
    public void TestingCards() {
        isP1Turn = true;
        playerOne.SetPlayerInTurn();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.IncrementMaxMana();
        playerOne.RefreshMana();
        HandController.Instance.SetPlayer(playerOne);
        playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));
        // playerOne.ServerAddCardToHand(DrawCardStatSO(playerOne));

        playerTwo.ServerAddCardToOpponentHand();
        playerTwo.ServerAddCardToOpponentHand();
        playerTwo.ServerAddCardToOpponentHand();
        playerTwo.ServerAddCardToOpponentHand();
        playerTwo.ServerAddCardToOpponentHand();
        // playerTwo.ServerAddCardToOpponentHand();
        // playerTwo.ServerAddCardToOpponentHand();
        // playerTwo.ServerAddCardToOpponentHand();
        // playerTwo.ServerAddCardToOpponentHand();
        // playerTwo.ServerAddCardToOpponentHand();
    }

    // Server functions for debugging purposes
    public int GetP1Health() {
        return playerOne.GetHp();
    }

    public int GetP2Health() {
        return playerTwo.GetHp();
    }

    public int GetP1Mana() {
        return playerOne.GetMana();
    }

    public int GetP2Mana() {
        return playerTwo.GetMana();
    }

    public int GetP1MaxMana() {
        return playerOne.GetTotalMana();
    }

    public int GetP2MaxMana() {
        return playerTwo.GetTotalMana();
    }

    public Player GetPlayerOne() {
        return playerOne;
    }

    public Player GetPlayerTwo() {
        return playerTwo;
    }
    // End of server debug functions
}
