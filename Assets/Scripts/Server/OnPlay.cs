using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Types;

public class OnPlay : MonoBehaviour
{
    public static OnPlay Instance;
    private void Awake() {
        Instance = this;
    }

    // public InPlayStats GetInPlayStatsFromCardStatsSO(CardStatsSO cardStatsSO) {
    //     InPlayStats inPlayStats = new InPlayStats
    //     {
    //         attacks = 0,
    //         totalAttacks = 1,
    //         attackVal = cardStatsSO.attack,
    //         hp = cardStatsSO.hp
    //     };

    //     return inPlayStats;
    // }
}
