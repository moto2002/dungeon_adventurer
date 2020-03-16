using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CharacterCreator
{
    const int BaseStartLife = 100;
    const int BaseStartMana = 50;

    public static Hero CreateHero()
    {
        var chars = ScriptableObject.CreateInstance<Hero>();
        chars.level = new Level(UnityEngine.Random.Range(1, 4), 0);
        chars.rarity = GetRarity();
        chars.race = (Race)Random.Range(0, 10);
        chars.displayName = DataHolder._data.NameGeneretor.GetNextRandomName();
        chars.fightClass = (FightClass)Random.Range(0, 15);
        chars.coreMain = SetStats(chars.fightClass, chars.rarity);
        chars.coreSub = new SubStats();
        chars.coreSub.manaRegen = 1;

        var minLife = (int)chars.race + chars.Main.con;
        chars.coreMaxLife = GetRandomLife(chars.fightClass, chars.level.currentLevel);
        chars.coreMaxMana = GetRandomMana(chars.fightClass, chars.level.currentLevel);
        chars.CurrentLife = chars.coreMaxLife;
        chars.CurrentMana = chars.coreMaxMana;
        chars.items = new ItemData[0];
        chars.position = 16;
        chars.skillInfos = GetSkillInfos(4, chars.fightClass);
        chars.ApplyItems();

        return chars;
    }

    static SkillInfo[] GetSkillInfos(int amount, FightClass fightClass)
    {
        var skillInfoList = new List<SkillInfo>();
        var skillPool = DataHolder._data.SkillsManager.GetCollectionByClass(fightClass).Skills.ToList();
        if (amount > skillPool.Count) throw new System.Exception($"Not enough Skills in Pool {fightClass.ToString()}");
        for (var i = 0; i < amount; i++)
        {
            var skill = skillPool[UnityEngine.Random.Range(0, skillPool.Count)];
            skillPool.Remove(skill);
            skillInfoList.Add(new SkillInfo(new Level(1, 0), skill.id));
        }
        return skillInfoList.ToArray();
    }

    static int GetRandomLife(FightClass fightClass, int level)
    {
        var life = CharactersService.LevelStats[fightClass].Life;
        return BaseStartLife + Random.Range(life.Item1, life.Item2) * level;
    }

    static int GetRandomMana(FightClass fightClass, int level)
    {
        var mana = CharactersService.LevelStats[fightClass].Mana;
        return BaseStartMana + Random.Range(mana.Item1, mana.Item2) * level;
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

    static MainStats SetStats(FightClass cla, Rarity rar)
    {
        var points = DataHolder._data.CharCreationDict[rar].startingPoints;
        var stats = new MainStats();
        float[] dist = CharactersService.LevelStats[cla].StatDistribution;
        stats.str = (int)(points * dist[0]);
        stats.con = (int)(points * dist[1]);
        stats.dex = (int)(points * dist[2]);
        stats.intel = (int)(points * dist[3]);
        stats.lck = (int)(points * dist[4]);

        return stats;
    }
}
