using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    private int numDots;

    private void Awake() {
        loadingText.text = "Loading";
        numDots = 0;
        InvokeRepeating("ChangeText", 0f, 1f);
    }

    private void ChangeText() 
    {
        loadingText.text = "Loading" + String.Concat(Enumerable.Repeat(".", numDots));
        numDots += 1;
        numDots %= 4;
    }
}
