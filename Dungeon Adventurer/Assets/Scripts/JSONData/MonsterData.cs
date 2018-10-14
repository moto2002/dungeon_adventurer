using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MonsterData
{
    public Monster[] monsters;
    public Dictionary<int, Monster> monstersDict;

    public void Instantiate()
    {
        monstersDict = new Dictionary<int, Monster>();
        foreach (Monster m in monsters)
            monstersDict.Add(m.id, m);  
    }
}

[System.Serializable]
public class Monster
{
    public int id;
    public string name;
    public int icon;
    public bool isBoss;
    public int experienceGain;
    public MonsterType type;
    public Item[] specialLoot;
    public int[] skills;
    public MainStats main;
    public SubStats sub;

}

public enum MonsterType
{
    Melee,
    Ranged,
    Support,
    Tank
}