using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    //public int id;
    public string itemName;
    public double price;
    public int level;
    public int powerLevel;
    public ItemType type;
    public Sprite icon;
    public Rarity rarity;
    public CraftingType smithed;
    public List<ItemMainStatValue> main;
    public List<ItemSubStatValue> sub;

    public BaseItem Base => _baseItem;

    ItemData _itemData;
    BaseItem _baseItem;

    public void SetData(ItemData data)
    {
        _itemData = data;
        _baseItem = DataHolder._data.BaseItems.GetItemById(data.baseItemID);
        itemName = data.itemName;
        type = _baseItem.Type;
        rarity = data.rarity;
        level = data.level;
        icon = _baseItem.GetIcon(level);
        smithed = CraftingType.Unsmithed;
        
        main = new List<ItemMainStatValue>();
        var power = 0;
        if (data.mainStatChanges.Length > 0)
            foreach (var value in data.mainStatChanges)
            {
                main.Add(value);
                power += value.value;
            }

        sub = new List<ItemSubStatValue>();
        if (data.subStatChanges.Length > 0)
            foreach (var value in data.subStatChanges)
            {
                sub.Add(value);
                power += value.value;
            }

        powerLevel = power;
        price = (priceByRarity[data.rarity] * power);
    }

    readonly Dictionary<Rarity, float> priceByRarity = new Dictionary<Rarity, float>() {
        { Rarity.Common, 4.3f},
        { Rarity.Uncommon, 21.8f },
        { Rarity.Magic, 32.8f },
        { Rarity.Rare, 53.0f },
        { Rarity.Epic, 86.3f },
        { Rarity.Legendary, 121.5f },
        { Rarity.Unique, 211.4f }
    };

    public void ApplyMainChanges(ItemMainStatValue[] mainChanges)
    {
        foreach (var change in mainChanges)
        {
            var changed = false;

            foreach (var value in main)
            {
                if (value.stat == change.stat)
                {
                    value.value += change.value;
                    changed = true;
                    break;
                }
            }

            if (!changed) main.Add(change);
        }
    }

    public void ApplySubChanges(ItemSubStatValue[] subChanges)
    {
        foreach (var change in subChanges)
        {
            var changed = false;

            foreach (var value in sub)
            {
                if (value.stat == change.stat)
                {
                    value.value += change.value;
                    changed = true;
                    break;
                }
            }

            if (!changed) sub.Add(change);
        }
    }

    public ItemData GetItemData()
    {
        return _itemData;
    }
}

[System.Serializable]
public class ItemMainStatValue
{
    public MainStat stat;
    public int value;

    public string StatToShortString()
    {

        switch (stat)
        {
            case MainStat.Strength:
                return "STR";
            case MainStat.Constitution:
                return "CON";
            case MainStat.Dexterity:
                return "DEX";
            case MainStat.Intelligence:
                return "INT";
            case MainStat.Luck:
                return "LCK";
        }
        return "ERROR";
    }

}

[System.Serializable]
public class ItemSubStatValue
{
    public SubStat stat = SubStat.None;
    public int value;

    public ItemSubStatValue() {}

    public ItemSubStatValue(SubStat subStat, int amount)
    {
        stat = subStat;
        value = amount;
    }
}

public enum ItemType
{
    Amulet,
    Belts,
    Boots,
    Bracers,
    Chest,
    Gloves,
    Helmet,
    Pants,
    Ring,
    Shoulder,
    Weapon
}

public enum CraftingType
{
    Unsmithed,
    Smithed,

}

