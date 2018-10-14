using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public Item[] items;
}

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public ItemType type;
    public int icon;
    public int rarity;
    public CraftingType smithed;
    public MainStats main;
    public SubStats sub;
}

public enum ItemType
{
    Amulet,
    Helmet,
    Chest,
    Shoulder,
    Belts,
    Gloves,
    Boots,
    Ring,
    Weapon
}

public enum CraftingType
{
    Unsmithed,
    Smithed,

}

