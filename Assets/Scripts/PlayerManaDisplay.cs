using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManaDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Player player;

    private void Start() {
        player.ManaConsumed += UpdateManaVisual;
        player.OnStartTurn += UpdateManaVisual;
        player.OnGameStart += UpdateManaVisual;
    }

    private void UpdateManaVisual(object sender, EventArgs e)
    {
        manaText.text = $"{player.GetRemainingMana()}/{player.GetTotalMana()}";
    }

}
