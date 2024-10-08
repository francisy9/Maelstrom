using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using CardTypes;

public class ServerManaManager : NetworkBehaviour
{
    public void InitializeManaDisplays() {
        GameManager.Instance.GetPlayerOne().TargetInitializeManaDisplay();
        GameManager.Instance.GetPlayerTwo().TargetInitializeManaDisplay();
    }

    [Server]
    public void IncrementAndRefreshPlayerMaxMana(Player player) {
        player.IncrementMaxMana();
        player.RefreshMana();
    }

    [Server]
    public void IncrementPlayerMana(Player player) {
        player.IncrementMaxMana();
    }

    [Server]
    public void RefreshPlayerMana(Player player) {
        player.RefreshMana();
    }

    [Server]
    public void ValidateSufficientManaToPlayCard(Player player, BaseCard card) {
        if (player.GetMana() < card.GetCurrentManaCost()) {
            Debug.LogError($"Player {player.name} does not have enough mana to play card {card.GetCardName()}");
        }
    }

    [Server]
    public void ConsumeManaForCardPlay(Player player, int manaCost) {
        player.ConsumeMana(manaCost);
    }

    public int GetP1Mana() => GameManager.Instance.GetPlayerOne().GetMana();
    public int GetP2Mana() => GameManager.Instance.GetPlayerTwo().GetMana();
    public int GetP1MaxMana() => GameManager.Instance.GetPlayerOne().GetMaxMana();
    public int GetP2MaxMana() => GameManager.Instance.GetPlayerTwo().GetMaxMana();
}
