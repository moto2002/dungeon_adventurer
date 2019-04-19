using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Custom/Monster")]
public class Monster : Character
{
    public Sprite icon;
    public bool isBoss;
    public int experienceGain;
    public int level;
    public MonsterType type;
    public Item[] specialLoot;

    public Monster(Monster mon) {

        id = mon.id;
        icon = mon.icon;
        isBoss = mon.isBoss;
        experienceGain = mon.experienceGain;
        type = mon.type;
        specialLoot = mon.specialLoot;
        name = mon.name;
        main = mon.main;
        sub = mon.sub;
        skills = mon.skills;
        position = mon.position;
        level = mon.level;
        maxLife = mon.maxLife;
        CurrentLife = mon.maxLife;
}

    public List<int> GetPossiblePositions()
    {
        switch (type)
        {
            case MonsterType.Melee:
                return new List<int> { 0, 1, 3, 4, 6, 7 };
            case MonsterType.Ranged:
                return new List<int> { 2, 5, 8 };
            case MonsterType.Support:
                return new List<int> { 1, 2, 4, 5, 7, 8 };
            case MonsterType.Tank:
                return new List<int> { 0, 3, 6 };
        }
        return new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    }
}

public enum MonsterType
{
    Melee,
    Ranged,
    Support,
    Tank
}