using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerHeroManager : NetworkBehaviour
{
    private Player player;
    [SerializeField] private Hero hero;
    [SerializeField] private EnemyHero enemyHero;

    public void Initialize(Player player) {
        this.player = player;
    }

    public Hero GetHero() {
        return hero;
    }

    public EnemyHero GetEnemyHero() {
        return enemyHero;
    }

    public void InitHeroes(int heroStartHp, int enemyHeroStartHp) {
        hero.InitHero(heroStartHp, player);
        enemyHero.InitHero(enemyHeroStartHp, player.GetOpponent());
    }

    public Vector3 GetHeroPosition() {
        return hero.transform.position;
    }

    public Vector3 GetEnemyHeroPosition() {
        return enemyHero.transform.position;
    }
}
