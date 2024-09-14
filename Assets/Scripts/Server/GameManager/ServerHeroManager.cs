using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ServerHeroManager : NetworkBehaviour
{
    [SerializeField] private int p1HeroMaxHp;
    [SerializeField] private int p2HeroMaxHp;

    [Server]
    public void InitializeHeroes() {
        ServerState.Instance.InitHeroes(p1HeroMaxHp, p2HeroMaxHp);
        GameManager.Instance.GetPlayerOne().TargetInitializeHeroes  (p1HeroMaxHp, p2HeroMaxHp);
        GameManager.Instance.GetPlayerTwo().TargetInitializeHeroes(p2HeroMaxHp, p1HeroMaxHp);
    }
}
