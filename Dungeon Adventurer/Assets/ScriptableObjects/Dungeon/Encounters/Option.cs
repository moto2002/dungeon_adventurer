using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Option
{
    readonly Dictionary<Rarity, float> _multiplierByRarity = new Dictionary<Rarity, float>()
    {
        { Rarity.Common, 1.0f},
        { Rarity.Uncommon, 1.2f},
        { Rarity.Magic, 1.4f},
        { Rarity.Rare, 1.6f},
        { Rarity.Epic, 1.8f},
        { Rarity.Legendary, 2f},
        { Rarity.Unique, 2.5f},
    };

    [Header("General")]
    public OptionRequirements[] requirements;
    public string startText;

    [Header("SuccessRate")]
    public OptionSuccessRate successRate;

    [Header("Result")]
    public string normalResultText;
    public OptionResult[] results;

    public void SetStats(int level, Rarity rarity)
    {
        foreach (var req in requirements)
        {
            req.SetStats(level * _multiplierByRarity[rarity]);
        }

        successRate.SetStats(level * _multiplierByRarity[rarity]);
    }
}

[Serializable]
public class OptionRequirements
{
    public MainStat stat;
    public int value;

    public int Value => (int)(value * _multi);

    float _multi;

    public void SetStats(float multi)
    {
        _multi = multi;
    }
}

[Serializable]
public class OptionSuccessRate
{
    public MainStat stat;
    [SerializeField] int minValue;
    [SerializeField] int desiredValue;
    [SerializeField] string successText;

    public bool NeedRoll => minValue != 0 && desiredValue != 0;
    public float Difference => DesiredValue - MinValue;
    public int MinValue => (int)(minValue * _multi);
    public int DesiredValue => (int)(desiredValue * _multi);
    public string SuccessText => successText;

    float _multi;

    public void SetStats(float multi)
    {
        _multi = multi;
    }
}

[Serializable]
public class OptionResult
{
    public enum ResultType
    {
        Action,
        Reward,
        Penalty,
    }

    public ResultType type;
    public Reward reward;
    public Penalty penalty;
    public UnityEvent action;

    public void Resolve(params Hero[] heroes)
    {
        switch (type)
        {
            case ResultType.Action:
                action?.Invoke();
                break;
            case ResultType.Penalty:
                foreach (var hero in heroes)
                {
                    penalty.Apply(hero);
                }
                break;
            case ResultType.Reward:
                foreach (var hero in heroes)
                {
                    reward.Apply(hero);
                }
                break;
        }
    }
}

[Serializable]
public class Reward
{
    public enum RewardType
    {
        Life,
        Mana,
        Exp,
        DungeonBuff,
        RoomBuff,
        BattleBuff
    }

    public RewardType type;
    [Header("Rewards")]
    public int value;
    public DungeonBuff dungeonBuff;
    public BattleBuff battleBuff;
    public RoomBuff roomBuff;

    public void Apply(Hero hero)
    {
        switch (type)
        {
            case RewardType.Life:
                hero.CurrentLife += value;
                break;
            case RewardType.Mana:
                hero.CurrentMana += value;
                break;
            case RewardType.Exp:
                hero.AddExperience(value);
                break;
            case RewardType.DungeonBuff:
                ServiceRegistry.Dungeon.ApplyBuff(dungeonBuff, hero);
                break;
            case RewardType.RoomBuff:
                ServiceRegistry.Dungeon.ApplyBuff(roomBuff, hero);
                break;
            case RewardType.BattleBuff:
                ServiceRegistry.Dungeon.ApplyBuff(battleBuff, hero);
                break;
        }
    }
}

[Serializable]
public class Penalty
{
    public enum PenaltyType
    {
        Life,
        Mana,
        IncreaseMainStat,
        IncreaseSubStat
    }

    public PenaltyType type;
    [Header("Penalty")]
    public int value;
    public MainStatChange mainBuff;
    public SubStatChange subBuff;

    public void Apply(Hero hero)
    {
        switch (type)
        {
            case PenaltyType.Life:
                hero.CurrentLife -= value;
                break;
            case PenaltyType.Mana:
                hero.CurrentMana -= value;
                break;
        }
    }
}