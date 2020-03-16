using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Custom/Monster")]
public class Monster : Character
{
    public Sprite icon;
    public MonsterType type;
    public bool isBoss;
    public int experienceGain;
    public int level;
    public FightingStyle fightingStyle;
    public Item[] specialLoot;
    public int minPhysicalDamage;
    public int maxPhysicalDamage;

    public void SetData(Monster mon)
    {
        id = mon.id;
        icon = mon.icon;
        isBoss = mon.isBoss;
        type = mon.type;
        experienceGain = mon.experienceGain;
        fightingStyle = mon.fightingStyle;
        specialLoot = mon.specialLoot;
        displayName = mon.displayName;
        coreMain = mon.Main;
        coreSub = mon.Sub;
        coreSkills = mon.Skills;
        position = mon.position;
        minPhysicalDamage = mon.minPhysicalDamage;
        maxPhysicalDamage = mon.maxPhysicalDamage;
    }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
        coreMaxLife = (int)(Sub.hp * level);
        CurrentLife = coreMaxLife;
    }

    public int GetExperience()
    {
        return experienceGain * level;
    }

    public List<int> GetPossiblePositions()
    {
        switch (fightingStyle)
        {
            case FightingStyle.Melee:
                return new List<int> { 0, 1, 3, 4, 6, 7 };
            case FightingStyle.Ranged:
                return new List<int> { 2, 5, 8 };
            case FightingStyle.Support:
                return new List<int> { 1, 2, 4, 5, 7, 8 };
            case FightingStyle.Tank:
                return new List<int> { 0, 3, 6 };
        }
        return new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    }
}

public enum FightingStyle
{
    Melee,
    Ranged,
    Support,
    Tank
}

public enum MonsterType
{
    Skeleton,
    Goblin,
    Troll,
    Demon,
    Strange
}