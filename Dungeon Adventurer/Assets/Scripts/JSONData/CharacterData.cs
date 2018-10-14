using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public Character[] characters;
}

[System.Serializable]
public class Character
{
    public string name;
    public int experience;
    public Rarity rarity;
    public Race race;
    public FightClass fightClass;
    public int position;
    public Item[] items;
    public int[] skills;
    public MainStats main;
    public SubStats sub;

}

[System.Serializable]
public class MainStats
{
    public int str;
    public int con;
    public int dex;
    public int intel;
    public int lck;
}

[System.Serializable]
public class SubStats
{
    public int hp;
    public int armor;
    public int dodge;
    public int speed;
    public int physDmg;
    public int critDmg;
    public int magicDmg;
    public int critMagic;
    public int critChance;
    public int fireRes;
    public int iceRes;
    public int lightRes;
    public int weight;
}

public enum Race
{
    //Good faction
    Human,
    Elf,
    Dwarf,
    Gnome,
    Giant,
    //Bad faction
    Orc,
    Demon,
    Goblin,
    Kobold,
    Troll

}

public enum Rarity
{
    Common,
    Uncommon,
    Magic,
    Rare,
    Epic,
    Legendary,
    Unique
}

public enum FightClass
{
    Warrior,
    Tank,
    Ranger,
    Priest,
    Guardian
}
