using UnityEngine;
using static Types;

public class Hero : HeroBase {
    public override void InitHero (int hp) {
        base.InitHero(hp);
        gameObject.layer = LayerMask.NameToLayer(HERO_LAYER);
    }

    public override void Death()
    {
        throw new System.NotImplementedException();
    }
}
