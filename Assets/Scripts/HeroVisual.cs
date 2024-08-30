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
    }
}
