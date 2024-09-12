using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using CardTypes;

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

    public void StartGame() => turnManager.StartGame();

    public void CmdEndTurn(NetworkConnectionToClient sender = null) => networkingManager.CmdEndTurn(sender);
    public void CmdPlayUnitCard(int handIndex, int boardIndex, NetworkConnectionToClient sender = null) => networkingManager.CmdPlayUnitCard(handIndex, boardIndex, sender);
    public void CmdAttack(int boardIndex, int opponentBoardIndex, NetworkConnectionToClient sender = null) => networkingManager.CmdAttack(boardIndex, opponentBoardIndex, sender);
    public void CmdCastSpell(int handIndex, Targeting targeting, NetworkConnectionToClient sender = null) => networkingManager.CmdCastSpell(handIndex, targeting, sender);


    // Functions to test on server client
    public void TestingCards() {}

    public bool IsP1Turn() => turnManager.IsP1Turn();
    public bool IsCardAtBoardIndex(int boardIndex) => boardManager.IsCardAtBoardIndex(boardIndex);
    public Player GetPlayerOne() => playerManager.GetPlayerOne();
    public Player GetPlayerTwo() => playerManager.GetPlayerTwo();
    public Player GetInTurnPlayer() => playerManager.GetInTurnPlayer();
    public Player GetNextPlayer() => playerManager.GetNextPlayer();
    public Player GetOpposingPlayer(Player player) => playerManager.GetOpposingPlayer(player);
    public ServerTurnManager GetTurnManager() => turnManager;
    public ServerPlayerManager GetPlayerManager() => playerManager;
    public ServerManaManager GetManaManager() => manaManager;
    public ServerHandManager GetHandManager() => handManager;
    public ServerHeroManager GetHeroManager() => heroManager;
    public ServerNetworkingManager GetNetworkingManager() => networkingManager;
    public ServerBoardManager GetBoardManager() => boardManager;
}
