using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroVisual : MonoBehaviour
{
    [SerializeField] private Sprite heroSprite;
    [SerializeField] private Hero hero;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image heroImage;

    private void Start() {
        hpText.text = hero.GetTotalHp().ToString();
        heroImage.sprite = heroSprite;
    }
}
