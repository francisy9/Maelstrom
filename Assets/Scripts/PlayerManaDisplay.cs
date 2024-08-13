using System;
using TMPro;
using UnityEngine;

public class PlayerManaDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Player player;

    public void InitializeManaDisplay(Player player) {
        this.player = player;
        player.ManaConsumed += UpdateManaVisual;
        player.OnStartTurn += UpdateManaVisual;
        player.OnGameStart += UpdateManaVisual;
    }

    public void UpdateManaVisual(object sender, EventArgs e)
    {
        manaText.text = $"{player.GetRemainingMana()}/{player.GetTotalMana()}";
    }

}
