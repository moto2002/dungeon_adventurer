using UnityEngine;

public static class CharacterCreator {

    // stats 4:2:2:1:1

    //str/con/dex/int/lck

    static readonly float[] WARRIOR_DISTRIBUTION = { 0.4f, 0.2f, 0.2f, 0.1f, 0.1f };
    static readonly float[] RANGER_DISTRIBUTION = { 0.1f, 0.2f, 0.4f, 0.1f, 0.2f };
    static readonly float[] TANK_DISTRIBUTION = { 0.2f, 0.4f, 0.2f, 0.1f, 0.1f };
    static readonly float[] PRIEST_DISTRIBUTION = { 0.1f, 0.2f, 0.1f, 0.4f, 0.2f };
    static readonly float[] GUARDIAN_DISTRIBUTION = { 0.1f, 0.1f, 0.2f, 0.4f, 0.2f };

    public static Hero CreateHero() {

        var chars = new Hero();
        chars.experience = 0;
        chars.rarity = GetRarity();
        Debug.Log("Rarity" + chars.rarity.ToString());
        chars.race = (Race)Random.Range(0, 10);
        chars.name = DataHolder._data.NameGeneretor.GetNextRandomName();
        chars.fightClass = (FightClass)Random.Range(0, 5);
        chars.main = SetStats(chars.fightClass, chars.rarity);
        var minLife = (int)chars.race + chars.main.con;
        chars.maxLife = Random.Range(minLife, minLife + chars.main.con);

        return chars;
    }

    static Rarity GetRarity() {

        var rand = Random.value;

        if (rand < 0.01f) {
            return Rarity.Unique;
        } else if (rand < 0.05f) {
            return Rarity.Legendary;
        } else if (rand < 0.15f) {
            return Rarity.Epic;
        } else if (rand < 0.4f) {
            return Rarity.Rare;
        } else if (rand < 0.6f) {
            return Rarity.Magic;
        } else if (rand < 0.8f) {
            return Rarity.Uncommon;
        } else {
            return Rarity.Common;
        }
    }

    static MainStats SetStats(FightClass cla, Rarity rar) {

        var points = DataHolder._data.CharCreationDict[rar].startingPoints;
        var stats = new MainStats();
        float[] dist = null;
        switch (cla) {
            case FightClass.Warrior:
                dist = WARRIOR_DISTRIBUTION;
                break;
            case FightClass.Tank:
                dist = TANK_DISTRIBUTION;
                break;
            case FightClass.Ranger:
                dist = RANGER_DISTRIBUTION;
                break;
            case FightClass.Priest:
                dist = PRIEST_DISTRIBUTION;
                break;
            case FightClass.Guardian:
                dist = GUARDIAN_DISTRIBUTION;
                break;
        }
        stats.str = (int)(points * dist[0]);
        stats.con = (int)(points * dist[1]);
        stats.dex = (int)(points * dist[2]);
        stats.intel = (int)(points * dist[3]);
        stats.lck = (int)(points * dist[4]);

        return stats;
    }
}
