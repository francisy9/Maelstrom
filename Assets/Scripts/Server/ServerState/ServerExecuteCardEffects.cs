using Mirror;
using UnityEngine;
using CardTypes;
using System;
using static Constants.Constants;
using ResponseTypes;
using System.Collections.Generic;

public class ServerExecuteCardEffects : NetworkBehaviour
{
    [Server]
    public CardEffectResponse ExecuteCardEffect(CardEffect effect, Targeting targeting, Player player, CardType cardType, int? handIndex = null, int? boardIndex = null, int additionalDamage = 0) {
        if (cardType != CardType.Spell && additionalDamage != 0) {
            Debug.LogError("Spell damage should be handled separately");
        }
        int originBoardIndex = boardIndex != null ? boardIndex.Value : HERO_BOARD_INDEX;

        ValidateTargeting(effect, targeting);
        Dictionary<TargetType, int[]> targetIndices = ServerState.Instance.GetBoardStateManager().GetBoardIndicesForTargetSelection(effect, targeting, player, originBoardIndex);
        switch (effect.effectType) {
            case EffectType.Buff:
                ApplyBuffEffect(effect, targetIndices, player, cardType, handIndex, boardIndex);
                break;
            case EffectType.Damage:
                ApplyDamageEffect(effect, targetIndices, player, cardType, handIndex, boardIndex, additionalDamage);
                break;
            case EffectType.Change:
                ApplyChangeEffect(effect, targetIndices, player, cardType, handIndex, boardIndex);
                break;
            case EffectType.Convert:
                ApplyConvertEffect(effect, targetIndices, player, cardType, handIndex, boardIndex);
                break;
            case EffectType.Heal:
                ApplyHealEffect(effect, targetIndices, player, cardType, handIndex, boardIndex);
                break;
            case EffectType.Spawn:
                ApplySpawnEffect(effect, targetIndices, player, cardType, handIndex, boardIndex);
                break;
            default:
                Debug.LogError($"Unimplemented effect type {effect.effectType}");
                break;
        }

        Dictionary<int, byte[]> requestingPlayerUnits = ServerState.Instance.GetBoardStateManager().GetCardEffectResponseUnits(targetIndices, player, true);
        Dictionary<int, byte[]> opposingPlayerUnits = ServerState.Instance.GetBoardStateManager().GetCardEffectResponseUnits(targetIndices, ServerState.Instance.GetOpponentPlayer(player), false);

        Debug.Log($"Requesting player units {string.Join(", ", requestingPlayerUnits.Keys)}");
        Debug.Log($"Opposing player units {string.Join(", ", opposingPlayerUnits.Keys)}");

        return new CardEffectResponse {
            AnimationStructure = new AnimationStructure {
                AnimationId = effect.animationId,
                OriginBoardIndex = originBoardIndex,
            },
            AlliedUnits = requestingPlayerUnits,
            EnemyUnits = opposingPlayerUnits,
        };
    }

    private void ApplyBuffEffect(CardEffect effect, Dictionary<TargetType, int[]> targetIndices, Player player, CardType cardType, int? handIndex, int? boardIndex) {
        throw new NotImplementedException();
    }

    private void ApplyDamageEffect(CardEffect effect, Dictionary<TargetType, int[]> targetIndices, Player player, CardType cardType, int? handIndex, int? boardIndex, int additionalDamage) {
        switch (effect.effectTargetType) {
            case EffectTargetType.Single:
                ValidateSingleTargeting(targetIndices);
                ApplySingleDamageEffect(effect, targetIndices, player, additionalDamage);
                break;
            case EffectTargetType.Self:
                ValidateSingleTargeting(targetIndices);
                ApplySingleDamageEffect(effect, targetIndices, player, additionalDamage);
                break;
            case EffectTargetType.Multiple:
            case EffectTargetType.OwnBoard:
            case EffectTargetType.EnemyBoard:
            case EffectTargetType.WholeBoard:
                ApplyMultipleDamageEffect(effect, targetIndices, player);
                break;
            default:
                Debug.LogError($"Unimplemented target type {effect.effectTargetType}");
                break;
        }
    }

    private void ApplyChangeEffect(CardEffect effect, Dictionary<TargetType, int[]> targeting, Player player, CardType cardType, int? handIndex, int? boardIndex) {
        throw new NotImplementedException();
    }
    private void ApplyConvertEffect(CardEffect effect, Dictionary<TargetType, int[]> targeting, Player player, CardType cardType, int? handIndex, int? boardIndex) {
        throw new NotImplementedException();
    }

    private void ApplyHealEffect(CardEffect effect, Dictionary<TargetType, int[]> targeting, Player player, CardType cardType, int? handIndex, int? boardIndex) {
        throw new NotImplementedException();
    }

    private void ApplySpawnEffect(CardEffect effect, Dictionary<TargetType, int[]> targeting, Player player, CardType cardType, int? handIndex, int? boardIndex) {
        throw new NotImplementedException();
    }



    // Helper functions
    private void ApplySingleDamageEffect(CardEffect effect, Dictionary<TargetType, int[]> targeting, Player player, int additionalDamage = 0) {
        foreach (var kvp in targeting) {

            if (kvp.Value[0] == HERO_BOARD_INDEX) {
                if (kvp.Key == TargetType.Ally) {
                    ServerState.Instance.ReduceHp(ServerState.Instance.GetHeroStats(player), effect.damageVal + additionalDamage);
                } else {
                    ServerState.Instance.ReduceHp(ServerState.Instance.GetOpponentHeroStats(player), effect.damageVal + additionalDamage);
                }
            } else {
                UnitCardStats targetStats = ServerState.Instance.GetBoardStateManager().GetSingleUnitCardStatsFromTargeting(targeting, player);
                ServerState.Instance.ReduceHp(targetStats, effect.damageVal + additionalDamage);
            }
        }
    }

    private void ApplyMultipleDamageEffect(CardEffect effect, Dictionary<TargetType, int[]> targetIndices, Player player, int additionalDamage = 0) {
        foreach (var kvp in targetIndices) {
            if (kvp.Key == TargetType.Ally) {
                UnitCardStats[] targets = ServerState.Instance.GetBoardStateManager().GetMultipleUnitCardStatsFromTargeting(kvp.Value, player);
                foreach (UnitCardStats target in targets) {
                    ServerState.Instance.ReduceHp(target, effect.damageVal + additionalDamage);
                }
            } else {
                UnitCardStats[] targets = ServerState.Instance.GetBoardStateManager().GetMultipleUnitCardStatsFromTargeting(kvp.Value, ServerState.Instance.GetOpponentPlayer(player));
                foreach (UnitCardStats target in targets) {
                    ServerState.Instance.ReduceHp(target, effect.damageVal + additionalDamage);
                }
            }
        }
    }

    private void ValidateTargeting(CardEffect effect, Targeting targeting) {
        if (effect.targetSelection == TargetSelection.UserSelect) {
            if (targeting == null) {
                Debug.LogError("User select targeting cannot be null");
            }
        }
    }

    private void ValidateSingleTargeting(Dictionary<TargetType, int[]> targetIndices) {
        if (targetIndices.Count != 1) {
            Debug.LogError("Single targeting should have only one target type");
            return;
        }

        foreach (var kvp in targetIndices) {
            if (kvp.Value.Length != 1) {
                Debug.LogError("Single targeting should have only one target index");
                return;
            }
        }
    }
}
