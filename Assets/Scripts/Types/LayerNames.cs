using UnityEngine;

namespace Layers
{
    public static class Layers
    {
        public const string PLAYER_PLAYED_CARD_LAYER = "PlayedCard";
        public const string OPPONENT_PLAYED_CARD_LAYER = "OpponentPlayedCard";
        public const string IN_HAND_CARD_LAYER = "InHandCard";
        public const string HERO_LAYER = "Hero";
        public const string OPPONENT_HERO_LAYER = "OpponentHero";

        public static LayerMask GetLayerMaskByTargetable(Targetable targetable) {
            LayerMask opponentCardLayerMask = LayerMask.GetMask(OPPONENT_PLAYED_CARD_LAYER);
            LayerMask opponentHeroLayerMask = LayerMask.GetMask(OPPONENT_HERO_LAYER);
            LayerMask playerCardLayerMask = LayerMask.GetMask(PLAYER_PLAYED_CARD_LAYER);
            LayerMask playerHeroLayerMask = LayerMask.GetMask(HERO_LAYER);
            int layerMask;
            switch (targetable) {
                case Targetable.AllEnemies:
                    layerMask = opponentCardLayerMask | opponentHeroLayerMask;
                    break;
                case Targetable.AllAllies:
                    layerMask = playerCardLayerMask | playerHeroLayerMask;
                    break;
                case Targetable.AllyUnits:
                    layerMask = playerCardLayerMask;
                    break;
                case Targetable.EnemyUnits:
                    layerMask = opponentCardLayerMask;
                    break;
                case Targetable.All:
                    layerMask = opponentCardLayerMask | opponentHeroLayerMask | playerCardLayerMask | playerHeroLayerMask;
                    break;
                default:
                    Debug.LogError("Invalid targetable type");
                    return LayerMask.GetMask();
            }
            return layerMask;
        }

    }
    
}
