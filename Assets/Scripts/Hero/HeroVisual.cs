using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroVisual : MonoBehaviour
{
    [SerializeField] private HeroBase hero;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image heroImage;

    public void InitVisual() {
        hpText.text = hero.GetTotalHp().ToString();
        hero.OnChange += Hero_OnChange;
    }

    private void Hero_OnChange(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual() {
        int hp = hero.GetRemainingHp();
        hpText.text = hp.ToString();

        if (hp < hero.GetTotalHp()) {
            hpText.color = Color.red;
        }
    }
}
