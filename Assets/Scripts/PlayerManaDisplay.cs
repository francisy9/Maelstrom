using System;
using TMPro;
using UnityEngine;

public class PlayerManaDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private Player player;
    [SerializeField] private GameManager gameManager;

    public void InitializeManaDisplay(Player player) {
        this.player = player;
    }

    public void UpdateManaVisual()
    {
        manaText.text = $"{player.GetMana()}/{player.GetMaxMana()}";
    }

}
