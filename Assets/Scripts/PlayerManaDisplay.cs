using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerManaDisplay : MonoBehaviour
{
    [SerializeField] private Image manaIcon;
    [SerializeField] private Image noManaIcon;
    [SerializeField] private Transform manaBarParent;
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI manaText;

    private List<Image> manaBar = new List<Image>();

    public void InitializeManaDisplay(Player player)
    {
        this.player = player;
        Debug.Log("ManaDisplay initialized with player: " + player.name);
    }

    public void UpdateManaVisual()
    {
        int currentMana = player.GetMana();
        int totalMana = player.GetTotalMana();

        // Adjust the number of mana icons
        ResizeManaBar(totalMana);

        // Update the visuals of each mana icon
        for (int i = 0; i < totalMana; i++)
        {
            manaBar[i].sprite = (i < currentMana) ? manaIcon.sprite : noManaIcon.sprite;
        }

        // Update the mana text
        manaText.text = $"{currentMana}/{totalMana}";
    }

    private void ResizeManaBar(int totalMana)
    {
        // Add more mana icons if needed
        while (manaBar.Count < totalMana)
        {
            Image newIcon = Instantiate(manaIcon, manaBarParent);
            manaBar.Add(newIcon);
        }

        // Remove excess mana icons if needed
        while (manaBar.Count > totalMana)
        {
            Image lastIcon = manaBar[manaBar.Count - 1];
            manaBar.RemoveAt(manaBar.Count - 1);
            Destroy(lastIcon.gameObject);
        }
    }
}

