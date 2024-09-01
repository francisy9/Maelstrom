using UnityEngine;
using static Types;

public class EnemyHero : HeroBase
{
    public static EnemyHero Instance;
    public override void InitHero (int hp) {
        base.InitHero(hp);
        gameObject.layer = LayerMask.NameToLayer(OPPONENT_HERO_LAYER);
        Instance = this;
        heroVisual.InitVisual();
    }

    public override void Death()
    {
        throw new System.NotImplementedException();
    }
}
