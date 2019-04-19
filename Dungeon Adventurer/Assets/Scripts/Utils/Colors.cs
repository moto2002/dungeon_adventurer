using UnityEngine;

public static class Colors
{
    static readonly Color RARITY_COMMON = Color.white;
    static readonly Color RARITY_UNCOMMON = Color.green;
    static readonly Color RARITY_MAGIC = Color.blue;
    static readonly Color RARITY_RARE = Color.yellow;
    static readonly Color RARITY_EPIC = Color.magenta;
    static readonly Color RARITY_LEGENDARY = new Color(0.92f, 0.84f, 0.14f, 1.0f);
    static readonly Color RARITY_UNIQUE = Color.red;


    static readonly Color MAIN_STRENGTH = new Color(0.6886792f, 0.1f, 0.1f, 1f);
    static readonly Color MAIN_CONSTITUTION = new Color(0.58f, 0.4f, 0.07f, 1f);
    static readonly Color MAIN_DEXTERITY = new Color(0.23f, 0.5f, 0.14f, 1f);
    static readonly Color MAIN_INTELLIGENCE = new Color(0.14f, 0.37f, 0.7f, 1f);
    static readonly Color MAIN_LUCK = new Color(0.29f, 0.29f, 0.29f, 1f);

    public static readonly Color BATTLE_ACTIVE = new Color(0.3f, 0.7f, 0.3f, 1f);
    public static readonly Color TARGET_HIGHLIGHT = new Color(0.8f, 0, 0, 1f);

    public static Color ByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return RARITY_COMMON;
            case Rarity.Uncommon:
                return RARITY_UNCOMMON;
            case Rarity.Magic:
                return RARITY_MAGIC;
            case Rarity.Rare:
                return RARITY_RARE;
            case Rarity.Epic:
                return RARITY_EPIC;
            case Rarity.Legendary:
                return RARITY_LEGENDARY;
            case Rarity.Unique:
                return RARITY_UNIQUE;
        }
        return Color.black;
    }

    public static Color ByMainStat(MainStat stat)
    {
        Debug.Log(stat.ToString());
        switch (stat)
        {
            case MainStat.Strength:
                return MAIN_STRENGTH;
            case MainStat.Constitution:
                return MAIN_CONSTITUTION;
            case MainStat.Dexterity:
                return MAIN_DEXTERITY;
            case MainStat.Intelligence:
                return MAIN_INTELLIGENCE;
            case MainStat.Luck:
                return MAIN_LUCK;
        }
        return Color.black;
    }
}