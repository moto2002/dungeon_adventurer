using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemType type;
    public Sprite icon;
    public Rarity rarity;
    public CraftingType smithed;
    public List<ItemMainStatValue> main;
    public List<ItemSubStatValue> sub;

    public void ApplyMainChanges(ItemMainStatValue[] mainChanges) {

        foreach (var change in mainChanges) {

            var changed = false;

            foreach (var value in main) {

                if (value.stat == change.stat) {
                    value.value += change.value;
                    changed = true;
                    continue;
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
                    continue;
                }
            }

            if (!changed) sub.Add(change);
        }
    }
}

[System.Serializable]
public class ItemMainStatValue
{
    public MainStat stat;
    public int value;

    public string StatToShortString() {

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
    public SubStat stat;
    public int value;
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

