using UnityEngine;
using static Types;

public class Hero : HeroBase {
    public static Hero Instance;
    public override void InitHero (int hp) {
        base.InitHero(hp);
        gameObject.layer = LayerMask.NameToLayer(HERO_LAYER);
        Instance = this;
        heroVisual.InitVisual();
    }

    public override void Death()
    {
        throw new System.NotImplementedException();
    }
}
