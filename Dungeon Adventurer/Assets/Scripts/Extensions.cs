using System.Linq;

public static class Extensions
{

    public static MainStats AddTo(this MainStats stats1, MainStats stats2)
    {
        var rv = new MainStats();
        rv.con = stats1.con + stats2.con;
        rv.str = stats1.str + stats2.str;
        rv.dex = stats1.dex + stats2.dex;
        rv.intel = stats1.intel + stats2.intel;
        rv.lck = stats1.lck + stats2.lck;
        return rv;
    }

    public static MainStats AddMainStatChange(this MainStats stats1, ItemMainStatValue change)
    {
        switch (change.stat)
        {
            case MainStat.Strength:
                stats1.str += change.value;
                break;
            case MainStat.Constitution:
                stats1.con += change.value;
                break;
            case MainStat.Dexterity:
                stats1.dex += change.value;
                break;
            case MainStat.Intelligence:
                stats1.intel += change.value;
                break;
            case MainStat.Luck:
                stats1.lck += change.value;
                break;
        }
        return stats1;
    }

    public static SubStats AddTo(this SubStats stats1, SubStats stats2)
    {
        var rv = new SubStats();
        rv.hp = stats1.hp + stats2.hp;
        rv.mana = stats1.mana + stats2.mana;
        rv.healthRegen = stats1.healthRegen + stats2.healthRegen;
        rv.manaRegen = stats1.manaRegen + stats2.manaRegen;
        rv.armor = stats1.armor + stats2.armor;
        rv.dodge = stats1.dodge + stats2.dodge;
        rv.speed = stats1.speed + stats2.speed;
        rv.physDmg = stats1.physDmg + stats2.physDmg;
        rv.critDmg = stats1.critDmg + stats2.critDmg;
        rv.magicDmg = stats1.magicDmg + stats2.magicDmg;
        rv.critMagic = stats1.critMagic + stats2.critMagic;
        rv.critChance = stats1.critChance + stats2.critChance;
        rv.fireRes = stats1.fireRes + stats2.fireRes;
        rv.iceRes = stats1.iceRes + stats2.iceRes;
        rv.lightRes = stats1.lightRes + stats2.lightRes;
        rv.weight = stats1.weight + stats2.weight;

        return rv;
    }

    public static SubStats AddSubStatChange(this SubStats stats1, ItemSubStatValue change)
    {
        switch (change.stat)
        {
            case SubStat.Health:
                stats1.hp += change.value;
                break;
            case SubStat.Mana:
                stats1.mana += change.value;
                break;
            case SubStat.HealthRegen:
                stats1.healthRegen += change.value;
                break;
            case SubStat.ManaRegen:
                stats1.manaRegen += change.value;
                break;
            case SubStat.Armor:
                stats1.armor += change.value;
                break;
            case SubStat.Dodge:
                stats1.dodge += change.value;
                break;
            case SubStat.Speed:
                stats1.speed += change.value;
                break;
            case SubStat.PhysicalDamage:
                stats1.physDmg += change.value;
                break;
            case SubStat.CriticalDamage:
                stats1.critDmg += change.value;
                break;
            case SubStat.MagicalDamage:
                stats1.magicDmg += change.value;
                break;
            case SubStat.CriticalMagic:
                stats1.critMagic += change.value;
                break;
            case SubStat.CriticalChance:
                stats1.critChance += change.value;
                break;
            case SubStat.FireResistance:
                stats1.fireRes += change.value;
                break;
            case SubStat.IceResistance:
                stats1.iceRes += change.value;
                break;
            case SubStat.LightningResistance:
                stats1.lightRes += change.value;
                break;
            case SubStat.Weight:
                stats1.weight += change.value;
                break;
        }
        return stats1;
    }

    public static ItemSubStatValue[] Scale(this ItemSubStatValue[] stats1, float value)
    {
        var rv = new ItemSubStatValue[stats1.Length];
        for (int i = 0; i < stats1.Length; i++)
        {
            rv[i] = new ItemSubStatValue(stats1[i].stat, (int)(stats1[i].value * value));
        }
        return rv;
    }
}
