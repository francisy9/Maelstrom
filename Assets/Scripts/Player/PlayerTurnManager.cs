using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;
public class PlayerTurnManager : NetworkBehaviour
{
    private Player player;
    [SyncVar] private bool inTurn;
    public event EventHandler OnStartTurn;
    [SerializeField] private Button endTurnButton;


    public void Initialize(Player player) {
        this.player = player;
        endTurnButton.onClick.AddListener(() => {
            RequestEndTurn();
        });
    }

    [TargetRpc]
    public void TargetStartTurn() {
        inTurn = true;
        OnStartTurn?.Invoke(this, EventArgs.Empty);
    }

    [TargetRpc]
    public void TargetEndTurn() {
        (player.GetBoard() as Board).ResetCardAttacks();
        inTurn = false;
    }

    [TargetRpc]
    public void TargetStartOpponentTurn() {
        inTurn = false;
    }

    public bool IsInTurn() {
        return inTurn;
    }

    private void RequestEndTurn() {
        GameManager.Instance.CmdEndTurn();
    }
}
