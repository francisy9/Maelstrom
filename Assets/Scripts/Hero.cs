using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : CanAttackBase {
    [SerializeField] private int totalHp;

    public int GetTotalHp() {
        return totalHp;
    }
    public override void Death() {}
}
