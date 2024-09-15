using UnityEngine;
using static Layers.Layers;

public class EnemyHero : HeroBase
{
    public static EnemyHero Instance;
    public override void InitHero (int hp, Player player) {
        base.InitHero(hp, player);
        gameObject.layer = LayerMask.NameToLayer(OPPONENT_HERO_LAYER);
        Instance = this;
        heroVisual.InitVisual();
    }

    public override void Death()
    {
        throw new System.NotImplementedException();
    }
}
