using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Text;
using CardTypes;
using static Constants.Constants;
using ResponseTypes;
public class ServerState : NetworkBehaviour
{
    [SerializeField] private ServerExecuteCardEffects serverExecuteCardEffects;
    public static ServerState Instance;
    private GameManager gameManager;
    private Player playerOne;
    private HeroStats p1Hero;
    private Player playerTwo;
    private HeroStats p2Hero;
    [SerializeField] private ServerHandStateManager handStateManager;
    [SerializeField] private ServerBoardStateManager boardStateManager;

    public override void OnStartServer() {
        Instance = this;
        gameManager = GameManager.Instance;
        InitComponents();
        Debug.Log("Server Update Instance created");
    }

    private void InitComponents() {
        handStateManager.Initialize();
        boardStateManager.Initialize();

    }

    [Server]
    public void SetPlayerRefs(Player playerOne, Player playerTwo) {
        this.playerOne = playerOne;
        this.playerTwo = playerTwo;
    }

    [Server]
    public void InitHeroes(int p1Hp, int p2Hp) {
        p1Hero = new HeroStats(p1Hp);
        p2Hero = new HeroStats(p2Hp);
    }

    [Server]
    public void MoveUnitCardToBoard(Player player, int handIndex, UnitCardStats cardStats, int boardIndex) {
        boardStateManager.InsertCardStatsAtBoardIndex(boardIndex, cardStats, player);
        GetHand(player).RemoveAt(handIndex);
    }

    [Server]
    public void PrintServerGameState() {
        Debug.Log($"{GameManager.Instance.GetInTurnPlayer().name}'s turn " +
        $"p1: {p1Hero.CurrentHP}hp {GameManager.Instance.GetManaManager().GetP1Mana()}/{GameManager.Instance.GetManaManager().GetP1MaxMana()} mana" +
        $" p2: {p2Hero.CurrentHP}hp {GameManager.Instance.GetManaManager().GetP2Mana()}/{GameManager.Instance.GetManaManager().GetP2MaxMana()} mana");

        PrintHand(playerOne);

        StringBuilder p1BoardText = new StringBuilder();
        p1BoardText.Append("p1 board: ");
        foreach (UnitCardStats cardStats in GetBoard(playerOne)) {
            p1BoardText.Append(GetUnitCardInfoString(cardStats) + " ");
        }
        Debug.Log(p1BoardText);

        PrintHand(playerTwo);

        StringBuilder p2BoardText = new StringBuilder();
        p2BoardText.Append("p2 board: ");
        foreach (UnitCardStats cardStats in GetBoard(playerTwo)) {
            p2BoardText.Append(GetUnitCardInfoString(cardStats) + " ");
        }
        Debug.Log(p2BoardText);

    }

    private void PrintHand(Player player) {
        StringBuilder handText = new StringBuilder();
        handText.Append($"{player.name} hand: ");
        foreach (BaseCard card in GetHand(player)) {
            switch (card.CardType) {
                case CardType.Unit:
                    handText.Append(GetUnitCardInfoString(card as UnitCardStats) + " ");
                    break;
                case CardType.Spell:
                    handText.Append(GetSpellCardInfoString(card as SpellCardStats) + " ");
                    break;
                case CardType.Weapon:
                    handText.Append(GetWeaponCardInfoString(card as WeaponCardStats) + " ");
                    break;
                default:
                    Debug.LogError($"unimplemented card type {card.CardType} {card.CardName}");
                    break;
            }
        }
        Debug.Log(handText);
    }

    private string GetUnitCardInfoString(UnitCardStats cardStats) {
        return $"{cardStats.CardName} {cardStats.CurrentManaCost} cost, {cardStats.CurrentAttack} attack, {cardStats.CurrentHP} health";
    }

    private string GetSpellCardInfoString(SpellCardStats cardStats) {
        return $"{cardStats.CardName} {cardStats.CurrentManaCost} cost, {cardStats.CurrentSpellDamage} damage";
    }

    private string GetWeaponCardInfoString(WeaponCardStats cardStats) {
        return $"{cardStats.CardName} {cardStats.CurrentManaCost} cost, {cardStats.CurrentAttack} attack, {cardStats.CurrentDurability} durability";
    }

    [Server]
    public object[] Attack(int boardIndex, int opponentBoardIndex, Player player) {
        bool attackerIsCard = GameManager.Instance.IsValidBoardIndexForCard(boardIndex);
        bool targetIsCard = GameManager.Instance.IsValidBoardIndexForCard(opponentBoardIndex);

        if (attackerIsCard & targetIsCard) {
            UnitCardStats card;
            UnitCardStats opponentCard;

            card = GetCardStatsAtBoardIndex(boardIndex, player);
            opponentCard = GetCardStatsAtBoardIndex(opponentBoardIndex, GetOpponentPlayer(player));

            ExecuteAttack(card, opponentCard);
            RemoveDeadUnitsFromBoard(player, boardIndex);
            RemoveDeadUnitsFromBoard(GetOpponentPlayer(player), opponentBoardIndex);
            
            return new UnitCardStats[] {
                card, opponentCard
            };
        } else if (attackerIsCard & !targetIsCard) {
            UnitCardStats card;
            HeroStats opponentHeroStats = GetOpponentHeroStats(player);
            card = GetCardStatsAtBoardIndex(boardIndex, player);

            ExecuteAttack(card, opponentHeroStats);
            RemoveDeadUnitsFromBoard(player, boardIndex);

            return new object[] {
                card, opponentHeroStats
            };
        } else if (!attackerIsCard & targetIsCard) {
            HeroStats heroStats = GetHeroStats(player);
            UnitCardStats targetCard = GetCardStatsAtBoardIndex(opponentBoardIndex, GetOpponentPlayer(player));

            ExecuteAttack(heroStats, targetCard);
            RemoveDeadUnitsFromBoard(GetOpponentPlayer(player), opponentBoardIndex);

            return new object[] {
                heroStats, targetCard
            };
        } else {
            HeroStats heroStats = GetHeroStats(player);
            HeroStats opponentHeroStats = GetOpponentHeroStats(player);

            ExecuteAttack(heroStats, opponentHeroStats);

            return new object[] {
                heroStats, opponentHeroStats
            };
        }
    }

    [Server]
    public void EndTurn(Player player) {
        List<UnitCardStats> board = boardStateManager.GetBoard(player);
        HeroStats heroStats = GetHeroStats(player);

        foreach (UnitCardStats cardStats in board) {
            cardStats.NumAttacks = cardStats.TotalNumAttacks;
        }
        heroStats.NumAttacks = heroStats.TotalNumAttacks;
    }


    [Server]
    public void CastSpell(int handIndex, Targeting targeting, Player player) {
        SpellCardStats cardToBeCast = GetCardStatsAtHandIndex(handIndex, player) as SpellCardStats;
        int spellDamageAdditive = -1;
        foreach (var effect in cardToBeCast.cardEffects) {
            if (effect.effectType == EffectType.Damage && spellDamageAdditive == -1) {
            spellDamageAdditive = CheckForSpellAdditiveSpellDamageEffects(player);
            }
            CardEffectResponse response = serverExecuteCardEffects.ExecuteCardEffect(effect, targeting, player, CardType.Spell, handIndex, additionalDamage: spellDamageAdditive);
            GameManager.Instance.SendCardEffectResponse(player, response);
        }
    }

    [Server]
    private void ExecuteAttack<T, U>(T attacker, U target) where T : class where U : class {
        ConsumeAttack(attacker);
        ReduceHp(target, GetAttackValue(attacker));
        ReduceHp(attacker, GetAttackValue(target));
    }

    [Server]
    public void ReduceHp<T>(T target, int amount) where T : class {
        if (target is UnitCardStats) {
            UnitCardStats unitCardStats = target as UnitCardStats;
            unitCardStats.CurrentHP -= amount;
        } else if (target is HeroStats) {
            HeroStats heroStats = target as HeroStats;
            heroStats.CurrentHP -= amount;
        } else {
            Debug.LogError($"Unimplemented target type {target.GetType()}");
        }
    }
    
    [Server]
    public void RemoveDeadUnitsFromBoard() {
        RemoveDeadUnitsFromPlayerBoard(playerOne);
        RemoveDeadUnitsFromPlayerBoard(playerTwo);
    }

    private void RemoveDeadUnitsFromPlayerBoard(Player player) {
        List<UnitCardStats> board = GetBoard(player);
        board.RemoveAll(cardStats => cardStats.CurrentHP <= 0);
    }

    [Server]
    private void ConsumeAttack<T>(T target) where T : class {
        if (target is UnitCardStats) {
            UnitCardStats unitCardStats = target as UnitCardStats;
            unitCardStats.NumAttacks -= 1;
        } else if (target is HeroStats) {
            HeroStats heroStats = target as HeroStats;
            heroStats.NumAttacks -= 1;
        } else {
            Debug.LogError($"Unimplemented target type {target.GetType()}");
        }
    }

    [Server]
    private int GetAttackValue<T>(T target) where T : class {
        if (target is UnitCardStats) {
            UnitCardStats unitCardStats = target as UnitCardStats;
            return unitCardStats.CurrentAttack;
        } else if (target is HeroStats) {
            HeroStats heroStats = target as HeroStats;
            return heroStats.CurrentAttack;
        } else {
            Debug.LogError($"Unimplemented target type {target.GetType()}");
            return 0;
        }
    }

    private int CheckForSpellAdditiveSpellDamageEffects(Player player) {
        List<UnitCardStats> board = GetBoard(player);
        int spellDamageAdditive = 0;
        foreach (UnitCardStats cardStats in board) {
            foreach (var effect in cardStats.cardEffects) {
                if (effect.triggerTime == TriggerTime.None && effect.effectType == EffectType.SpellDamage) {
                    spellDamageAdditive += effect.damageVal;
                }
            }
        }
        return spellDamageAdditive;
    }

    [Server]
    public HeroStats GetHeroStats(Player player) {
        return player == playerOne ? p1Hero : p2Hero;
    }
    
    [Server]
    public HeroStats GetOpponentHeroStats(Player player) {
        return player == playerOne ? p2Hero : p1Hero;
    }

    [Server]
    public Player GetOpponentPlayer(Player player) {
        return player == playerOne ? playerTwo : playerOne;
    }

    public Player GetPlayerOne() => playerOne;
    public Player GetPlayerTwo() => playerTwo;
    private List<BaseCard> GetHand(Player player) => handStateManager.GetHand(player);
    private List<UnitCardStats> GetBoard(Player player) => boardStateManager.GetBoard(player);
    public void AddCardToHand(BaseCard baseCard, Player player) => handStateManager.AddCardToHand(baseCard, player);
    private List<UnitCardStats> GetOpponentBoard(Player player) => boardStateManager.GetOpponentBoard(player);
    public UnitCardStats GetCardStatsAtIndex(Player player, int index) => boardStateManager.GetCardStatsAtIndex(player, index);
    public bool CardExistAtIndices(Player player, int index) => boardStateManager.CardExistAtIndices(player, index);
    private void RemoveDeadUnitsFromBoard(Player player, int index) => boardStateManager.RemoveDeadUnitsFromBoard(player, index);
    public BaseCard GetCardStatsAtHandIndex(int i, Player player) => handStateManager.GetCardStatsAtHandIndex(i, player);
    public UnitCardStats GetCardStatsAtBoardIndex(int i, Player player) => boardStateManager.GetCardStatsAtBoardIndex(i, player);
    public ServerBoardStateManager GetBoardStateManager() => boardStateManager;
}
