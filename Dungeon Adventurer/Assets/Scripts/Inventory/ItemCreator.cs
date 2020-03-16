using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemCreator
{
    public static ItemData CreateItem(int level)
    {
        var baseItem = DataHolder._data.BaseItems.GetRandomItem();

        var data = new ItemData();

        data.level = level;
        data.baseItemID = baseItem.Id;
        data.rarity = GetRarity();
        data.skillID = 0;
        data.mainStatChanges = GetMainStats(data.rarity, level, baseItem.StatFocus[0].focus);
        data.subStatChanges = GetSubStats(data.rarity, level, baseItem.StatFocus[0].focus);
        data.itemName = $"{data.rarity.ToString()} {baseItem.Type.ToString()}";

        return data;
    }

    static Rarity GetRarity()
    {

        var rand = Random.value;

        if (rand < 0.01f)
        {
            return Rarity.Unique;
        } else if (rand < 0.05f)
        {
            return Rarity.Legendary;
        } else if (rand < 0.15f)
        {
            return Rarity.Epic;
        } else if (rand < 0.4f)
        {
            return Rarity.Rare;
        } else if (rand < 0.6f)
        {
            return Rarity.Magic;
        } else if (rand < 0.8f)
        {
            return Rarity.Uncommon;
        } else
        {
            return Rarity.Common;
        }
    }

    static readonly Dictionary<Rarity, float> mainStatsPerLevelByRarity = new Dictionary<Rarity, float>()
    {
        { Rarity.Common, 1f },
        { Rarity.Uncommon, 1.8f },
        { Rarity.Magic, 2.8f },
        { Rarity.Rare, 4.2f },
        { Rarity.Epic, 7.5f },
        { Rarity.Legendary, 11f },
        { Rarity.Unique, 15f }
    };

    static readonly Dictionary<Rarity, float> subStatsPerLevelByRarity = new Dictionary<Rarity, float>()
    {
        { Rarity.Common, 12f },
        { Rarity.Uncommon, 20f },
        { Rarity.Magic, 35f },
        { Rarity.Rare, 60f },
        { Rarity.Epic, 100f },
        { Rarity.Legendary, 150f },
        { Rarity.Unique, 200f }
    };


    static readonly Dictionary<Rarity, int> mainStatsByRarity = new Dictionary<Rarity, int>()
    {
        { Rarity.Common, 0 },
        { Rarity.Uncommon, 1 },
        { Rarity.Magic, 1 },
        { Rarity.Rare, 2 },
        { Rarity.Epic, 2 },
        { Rarity.Legendary, 3 },
        { Rarity.Unique, 3 }
    };

    static readonly Dictionary<Rarity, int> subStatsByRarity = new Dictionary<Rarity, int>()
    {
        { Rarity.Common, 1 },
        { Rarity.Uncommon, 1 },
        { Rarity.Magic, 1 },
        { Rarity.Rare, 2 },
        { Rarity.Epic, 2 },
        { Rarity.Legendary, 3 },
        { Rarity.Unique, 4 }
    };

    static readonly Dictionary<MainStat, SubStat[]> possibleSubStatsByMainStat = new Dictionary<MainStat, SubStat[]>()
    {
        { MainStat.Strength, new SubStat[] {SubStat.PhysicalDamage, SubStat.CriticalDamage, SubStat.Armor, SubStat.Health, SubStat.Armor, SubStat.HealthRegen } },
        { MainStat.Constitution, new SubStat[] {SubStat.Health, SubStat.Armor, SubStat.HealthRegen }  },
        { MainStat.Dexterity, new SubStat[] {SubStat.Dodge, SubStat.Speed, SubStat.CriticalChance, SubStat.CriticalDamage}  },
        { MainStat.Intelligence, new SubStat[] {SubStat.MagicalDamage, SubStat.CriticalMagic, SubStat.IceResistance, SubStat.FireResistance, SubStat.LightningResistance, SubStat.Mana, SubStat.ManaRegen } },
        { MainStat.Luck, new SubStat[] {SubStat.CriticalChance, SubStat.CriticalMagic, SubStat.CriticalDamage} }
    };

    static ItemMainStatValue[] GetMainStats(Rarity rarity, int level, MainStat[] prog)
    {
        var statFocus = prog.ToList();
        var statNorm = mainStatsByRarity[rarity];
        var stats = new ItemMainStatValue[Random.Range(statNorm, statNorm + 1)];
        var statPoints = mainStatsPerLevelByRarity[rarity] * level;
        for (int i = 0; i < stats.Length; i++)
        {
            var stat = new ItemMainStatValue();
            stat.stat = statFocus[0];
            statFocus.RemoveAt(0);
            var value = (int)(statPoints * (1 - (Random.Range(0f, 0.25f) * i)));
            if (value <= 0) value = 1;
            stat.value = value;
            stats[i] = stat;
        }
        return stats;
    }

    static ItemSubStatValue[] GetSubStats(Rarity rarity, int level, MainStat[] prog)
    {
        var statFocus = prog.ToList();
        var statNorm = subStatsByRarity[rarity];
        var stats = new List<ItemSubStatValue>(new ItemSubStatValue[Random.Range(statNorm, statNorm + 1)]);
        var statPoints = subStatsPerLevelByRarity[rarity] * level;
        for (int i = 0; i < stats.Count; i++)
        {
            var stat = new ItemSubStatValue();
            var possibleStats = possibleSubStatsByMainStat[statFocus[Random.Range(0, i)]];
            stat.stat = possibleStats[Random.Range(0, possibleStats.Length)];
            var value = (int)(statPoints * (1 - (Random.Range(0f, 0.25f) * i)));
            if (value <= 0) value = 1;
            stat.value = value;

            var posStat = stats.FirstOrDefault(e => e!=null && e.stat == stat.stat);
            if (posStat != null)
            {
                posStat.value += stat.value;
                stats.RemoveAt(i);
                i--;
            } else {
                stats[i] = stat;
            }
        }
        return stats.ToArray();
    }
}
