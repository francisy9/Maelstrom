using System.Collections.Generic;
using Mirror;
using UnityEngine;
using CardTypes;
using static Constants.Constants;
using System.Linq;
using System;

public class ServerBoardStateManager : NetworkBehaviour
{
    private List<UnitCardStats> p1Board;
    private List<UnitCardStats> p2Board;

    [Server]
    public void Initialize()
    {
        p1Board = new List<UnitCardStats>();
        p2Board = new List<UnitCardStats>();
    }

    [Server]
    public bool CardExistAtIndices(Player player, int index)
    {
        List<UnitCardStats> board = GetBoard(player);
        if (index < 0 || index >= board.Count)
        {
            return false;
        }

        if (index > board.Count - 1)
        {
            return false;
        }

        return true;
    }

    public List<UnitCardStats> GetBoard(Player player)
    {
        return player == ServerState.Instance.GetPlayerOne() ? p1Board : p2Board;
    }

    public List<UnitCardStats> GetOpponentBoard(Player player)
    {
        return player == ServerState.Instance.GetPlayerOne() ? p2Board : p1Board;
    }

    [Server]
    public UnitCardStats GetCardStatsAtIndex(Player player, int index)
    {
        return GetBoard(player)[index];
    }

    [Server]
    public void RemoveDeadUnitsFromBoard(Player player, int index)
    {
        if (index != HERO_BOARD_INDEX)
        {
            List<UnitCardStats> board = GetBoard(player);
            if (board[index].CurrentHP <= 0)
            {
                board.RemoveAt(index);
            }
        }
    }

    [Server]
    public UnitCardStats GetCardStatsAtBoardIndex(int i, Player player)
    {
        return GetBoard(player)[i];
    }

    [Server]
    public void InsertCardStatsAtBoardIndex(int i, UnitCardStats cardStats, Player player)
    {
        GetBoard(player).Insert(i, cardStats);
    }

    [Server]
    public Dictionary<TargetType, int[]> GetRandomBoardIndices(int numTarget, Targetable targetable, Player player, bool allowDuplicates)
    {
        System.Random random = new System.Random();
        List<int> availableAllyIndices = Enumerable.Range(0, GetBoard(player).Count).ToList();
        List<int> availableEnemyIndices = Enumerable.Range(0, GetOpponentBoard(player).Count).ToList();
        Dictionary<TargetType, int[]> result = new Dictionary<TargetType, int[]>();

        switch (targetable)
        {
            case Targetable.AllAllies:
                availableAllyIndices.Add(HERO_BOARD_INDEX);
                result[TargetType.Ally] = GetRandomIndices(availableAllyIndices, numTarget, allowDuplicates);
                break;
            case Targetable.AllEnemies:
                availableEnemyIndices.Add(HERO_BOARD_INDEX);
                result[TargetType.Enemy] = GetRandomIndices(availableEnemyIndices, numTarget, allowDuplicates);
                break;
            case Targetable.AllyUnits:
                result[TargetType.Ally] = GetRandomIndices(availableAllyIndices, numTarget, allowDuplicates);
                break;
            case Targetable.EnemyUnits:
                result[TargetType.Enemy] = GetRandomIndices(availableEnemyIndices, numTarget, allowDuplicates);
                break;
            case Targetable.All:
                availableAllyIndices.Add(HERO_BOARD_INDEX);
                availableEnemyIndices.Add(HERO_BOARD_INDEX);
                List<List<int>> allIndices = new List<List<int>> { availableAllyIndices, availableEnemyIndices };
                List<int> allyTargets = new List<int>();
                List<int> enemyTargets = new List<int>();

                for (int i = 0; i < numTarget; i++)
                {
                    int randomIndex = random.Next(allIndices.Count);
                    List<int> chosenIndices = allIndices[randomIndex];
                    if (chosenIndices.Count == 0) {
                        chosenIndices = allIndices[1 - randomIndex];
                    }

                    if (chosenIndices.Count == 0) {
                        break;
                    }
                    
                    int boardIndex = random.Next(chosenIndices.Count);
                    int selectedBoardIndex = chosenIndices[boardIndex];

                    if (randomIndex == 0) {
                        allyTargets.Add(selectedBoardIndex);
                    } else {
                        enemyTargets.Add(selectedBoardIndex);
                    }

                    if (!allowDuplicates)
                        chosenIndices.RemoveAt(selectedBoardIndex);
                }

                if (allyTargets.Count > 0)
                    result[TargetType.Ally] = allyTargets.ToArray();
                if (enemyTargets.Count > 0)
                    result[TargetType.Enemy] = enemyTargets.ToArray();
                break;
            default:
                Debug.LogError($"Unimplemented targetable {targetable}");
                break;
        }

        return result;
    }

    private int[] GetRandomIndices(List<int> indices, int numTarget, bool allowDuplicates)
    {
        System.Random random = new System.Random();
        List<int> selectedIndices = new List<int>();
        while (selectedIndices.Count < numTarget && indices.Count > 0)
        {
            int randomIndex = random.Next(indices.Count);
            selectedIndices.Add(indices[randomIndex]);
            if (!allowDuplicates)
            {
                indices.RemoveAt(randomIndex);
            }
        }
        return selectedIndices.ToArray();
    }

    public Dictionary<TargetType, int[]> GetBoardIndicesForTargetSelection(CardEffect effect, Targeting userSelectedTargeting, Player player, int originBoardIndex)
    {
        Dictionary<TargetType, int[]> result = new Dictionary<TargetType, int[]>();

        switch (effect.targetSelection)
        {
            case TargetSelection.None:
                switch (effect.effectTargetType)
                {
                    case EffectTargetType.Self:
                        result[TargetType.Ally] = new int[] { originBoardIndex };
                        break;
                    case EffectTargetType.Single:
                    case EffectTargetType.Multiple:
                        Debug.LogError("Single and Multiple not implemented without UserTargetSelection not implemented");
                        break;
                    case EffectTargetType.PlayerHero:
                        result[TargetType.Ally] = new int[] { HERO_BOARD_INDEX };
                        break;
                    case EffectTargetType.OpponentHero:
                        result[TargetType.Enemy] = new int[] { HERO_BOARD_INDEX };
                        break;
                    case EffectTargetType.OwnBoard:
                        result[TargetType.Ally] = GetAllAvailableBoardIndices(player, true);
                        break;
                    case EffectTargetType.EnemyBoard:
                        result[TargetType.Enemy] = GetAllAvailableBoardIndices(ServerState.Instance.GetOpponentPlayer(player), true);
                        break;
                    case EffectTargetType.WholeBoard:
                        result[TargetType.Ally] = GetAllAvailableBoardIndices(player, true);
                        result[TargetType.Enemy] = GetAllAvailableBoardIndices(ServerState.Instance.GetOpponentPlayer(player), true);
                        break;
                    default:
                        break;
                }
                break;
            case TargetSelection.RandomWithDuplicates:
            case TargetSelection.RandomWithoutDuplicates:
                result = GetRandomBoardIndices(effect.numTargets, effect.targetable, player, effect.targetSelection == TargetSelection.RandomWithDuplicates);
                break;
            case TargetSelection.Adjacent:
                List<int> indices = new List<int>();
                int leftIndex = originBoardIndex - 1;
                int rightIndex = originBoardIndex + 1;
                if (CardExistAtIndices(player, leftIndex))
                {
                    indices.Add(leftIndex);
                }
                if (CardExistAtIndices(player, rightIndex))
                {
                    indices.Add(rightIndex);
                }
                result[TargetType.Ally] = indices.ToArray();
                break;
            case TargetSelection.UserSelect:
                result[userSelectedTargeting.targetType] = userSelectedTargeting.targetBoardIndices;
                break;
            default:
                Debug.LogError($"Unhandled TargetSelection: {effect.targetSelection}");
                break;
        }
        return result;
    }

    private int[] GetAllAvailableBoardIndices(Player player, bool includeHero)
    {
        List<int> indices = new List<int>();
        indices.AddRange(Enumerable.Range(0, GetBoard(player).Count));
        if (includeHero)
        {
            indices.Add(HERO_BOARD_INDEX);
        }
        return indices.ToArray();
    }

    [Server]
    public UnitCardStats[] GetMultipleUnitCardStatsFromTargeting(int[] indices, Player player) {
        List<UnitCardStats> board = GetBoard(player);
        return indices
            .Where(i => i >= 0 && i <= MAX_BOARD_INDEX)
            .Select(i => board[i])
            .ToArray();
    }

    [Server]
    public UnitCardStats GetSingleUnitCardStatsFromTargeting(Dictionary<TargetType, int[]> targeting, Player player)
    {
        var targetPair = targeting.First();
        TargetType targetType = targetPair.Key;
        int index = targetPair.Value[0];

        List<UnitCardStats> board = targetType == TargetType.Ally ? GetBoard(player) : GetOpponentBoard(player);
        return board[index];
    }

    [Server]
    public Dictionary<int, byte[]> GetCardEffectResponseUnits(Dictionary<TargetType, int[]> targeting, Player player, bool isCaster)
    {
        Dictionary<int, byte[]> units = new Dictionary<int, byte[]>();

        if (isCaster) {
            if (!targeting.ContainsKey(TargetType.Ally)) {
                return units;
            }
        } else {
            if (!targeting.ContainsKey(TargetType.Enemy)) {
                return units;
            }
        }

        int[] indicesToCheck = isCaster ? targeting[TargetType.Ally] : targeting[TargetType.Enemy];
        foreach (int index in indicesToCheck) {
            if (index == HERO_BOARD_INDEX) {
                byte[] serializedHero = ServerState.Instance.GetHeroStats(player).Serialize();
                units.Add(index, serializedHero);
            } else {
                byte[] serializedUnit = GetCardStatsAtIndex(player, index).Serialize();
                units.Add(index, serializedUnit);
            }
        }
        return units;
    }
}
