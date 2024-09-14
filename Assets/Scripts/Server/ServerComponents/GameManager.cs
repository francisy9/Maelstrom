using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using CardTypes;
using ResponseTypes;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private ServerPlayerManager playerManager;
    [SerializeField] private ServerHandManager handManager;
    [SerializeField] private ServerTurnManager turnManager;
    [SerializeField] private ServerManaManager manaManager;
    [SerializeField] private ServerNetworkingManager networkingManager;
    [SerializeField] private ServerHeroManager heroManager;
    [SerializeField] private ServerBoardManager boardManager;
    
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

    // One time call by player manager to start game
    public void StartGame() => turnManager.StartGame();


    // Commands to be called by client
    public void CmdEndTurn(NetworkConnectionToClient sender = null) => networkingManager.CmdEndTurn(sender);
    public void CmdPlayUnitCard(int handIndex, int boardIndex, NetworkConnectionToClient sender = null) => networkingManager.CmdPlayUnitCard(handIndex, boardIndex, sender);
    public void CmdAttack(int boardIndex, int opponentBoardIndex, NetworkConnectionToClient sender = null) => networkingManager.CmdAttack(boardIndex, opponentBoardIndex, sender);
    public void CmdCastSpell(int handIndex, Targeting targeting, NetworkConnectionToClient sender = null) => networkingManager.CmdCastSpell(handIndex, targeting, sender);


    // Functions to test on server client
    public void TestingCards() {}

    public bool IsP1Turn() => turnManager.IsP1Turn();
    public bool IsValidBoardIndexForCard(int boardIndex) => boardManager.IsValidBoardIndexForCard(boardIndex);
    public Player GetPlayerOne() => playerManager.GetPlayerOne();
    public Player GetPlayerTwo() => playerManager.GetPlayerTwo();
    public Player GetInTurnPlayer() => playerManager.GetInTurnPlayer();
    public Player GetNextPlayer() => playerManager.GetNextPlayer();
    public Player GetOpposingPlayer(Player player) => playerManager.GetOpposingPlayer(player);

    // Getting server components
    public ServerTurnManager GetTurnManager() => turnManager;
    public ServerPlayerManager GetPlayerManager() => playerManager;
    public ServerManaManager GetManaManager() => manaManager;
    public ServerHandManager GetHandManager() => handManager;
    public ServerHeroManager GetHeroManager() => heroManager;
    public ServerNetworkingManager GetNetworkingManager() => networkingManager;
    public ServerBoardManager GetBoardManager() => boardManager;

    // Function invoked by ServerState
    public void SendCardEffectResponse(Player player, CardEffectResponse response) => networkingManager.ServerSendCardEffectResponse(player, response);
}
