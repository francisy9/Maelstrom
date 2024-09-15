using UnityEngine;
using static Layers.Layers;

public class Hero : HeroBase {
    public static Hero Instance;
    public override void InitHero (int hp, Player player) {
        base.InitHero(hp, player);
        gameObject.layer = LayerMask.NameToLayer(HERO_LAYER);
        Instance = this;
        heroVisual.InitVisual();
    }

    public override void Death()
    {
        throw new System.NotImplementedException();
    }
}
