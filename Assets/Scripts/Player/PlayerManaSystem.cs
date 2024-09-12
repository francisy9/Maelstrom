using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManaSystem : NetworkBehaviour
{
    private Player player;
    [SyncVar (hook = nameof(OnManaChange))]
    private int mana = 0;
    [SyncVar (hook = nameof(OnManaChange))]
    private int maxMana = 0;
    [SerializeField] private PlayerManaDisplay manaDisplay;
    [SerializeField] private PlayerManaDisplay enemyManaDisplay;
    

    public void Initialize(Player player) {
        this.player = player;
    }

    [TargetRpc]
    public void TargetInitializeManaDisplay() {
        if (manaDisplay == null || enemyManaDisplay == null) {
            Debug.LogError("Mana display is null");
        }
        manaDisplay.InitializeManaDisplay(player);
        enemyManaDisplay.InitializeManaDisplay(player.GetOpponent());
    }

    private void OnManaChange(int _, int _a) {
        manaDisplay.UpdateManaVisual();
        enemyManaDisplay.UpdateManaVisual();
    }

    [Server]
    public void IncrementMaxMana() {
        maxMana += 1;
    }

    [Server]
    public void ConsumeMana(int mana) {
        this.mana -= mana;
    }

    [Server]
    public void RefreshMana() {
        mana = maxMana;
    }

    public int GetMana() => mana;
    public int GetMaxMana() => maxMana;
}
