#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static DamageInfluence;
using static IncreaseTurnCount;

[System.Serializable]
public class CharacterData
{
    public Hero[] characters;

    public void AddHero(Hero hero)
    {
        var list = new List<Hero>(characters);
        var nextId = list.Count <= 0 ? 0 : list.Max(e => e.id);
        hero.id = ++nextId;
        list.Add(hero);
        characters = list.ToArray();
    }

    public void RemoveHero(Hero hero)
    {
        var list = new List<Hero>(characters);
        list.Remove(hero);
        characters = list.ToArray();
    }
}
public class CharDeathEvent : UnityEvent<int> { }
public class LifeChangeEvent : UnityEvent<int> { }
public class MarkerChangeEvent : UnityEvent<MarkerInfo> { }
public class DotChangeEvent : UnityEvent<DotInfo> { }
public class StatusChangeEvent : UnityEvent<StatusInfo> { }
public class BuffChangeEvent : UnityEvent<BuffInfo> { }

public class Character : ScriptableObject
{
    public int id;
    public string displayName;
    public int position;
    public MainStats coreMain;
    public SubStats coreSub;
    public int coreMaxLife;
    public int coreMaxMana;

    public Skill[] coreSkills;
    [NonSerialized]
    public MainStats itemsMain = new MainStats();
    [NonSerialized]
    public SubStats itemsSub = new SubStats();
    [NonSerialized]
    public SubStats combinedSub = new SubStats();
    [NonSerialized]
    public Skill[] itemsSkills;
    [NonSerialized]
    public int itemsMaxLife;

    public MainStats Main => coreMain.AddTo(itemsMain);
    public SubStats Sub => coreSub.AddTo(itemsSub).AddTo(combinedSub);
    public Skill[] Skills => coreSkills;
    public int MaxLife => coreMaxLife + Sub.Health;
    public int MaxMana => coreMaxMana + Sub.Mana;

    [SerializeField] int currentLife = 0;
    [SerializeField] int currentMana = 0;

    public int CurrentLife
    {
        get
        {
            return currentLife;
        }
        set
        {
            var oldvalue = currentLife;
            currentLife = Mathf.Min(value, MaxLife);
            var change = currentLife - oldvalue;
            if (change != 0) OnLifeChanged.Invoke(change);
            if (currentLife <= 0)
                OnCharacterDeath.Invoke(id);
        }
    }

    public int CurrentMana
    {
        get
        {
            return currentMana;
        }
        set
        {
            var oldvalue = currentMana;
            currentMana = Mathf.Min(value, MaxMana);
        }
    }


    Dictionary<StatusType, StatusInfo> _appliedStatuses = new Dictionary<StatusType, StatusInfo>();
    Dictionary<MarkerType, MarkerInfo> _appliedMarkers = new Dictionary<MarkerType, MarkerInfo>();
    Dictionary<SubStat, BuffInfo> _appliedBuffs = new Dictionary<SubStat, BuffInfo>();
    Dictionary<DoTDamageType, DotInfo> _appliedDots = new Dictionary<DoTDamageType, DotInfo>();

    [NonSerialized]
    public CharDeathEvent OnCharacterDeath = new CharDeathEvent();

    [NonSerialized]
    public DotChangeEvent OnDotsChanged = new DotChangeEvent();
    public void AddDot(DotInfo info)
    {
        if (_appliedDots.ContainsKey(info.Type))
        {
            _appliedDots[info.Type].AddDotInfo(info);
            info = _appliedDots[info.Type];
        }
        else
            _appliedDots.Add(info.Type, info);
        OnDotsChanged.Invoke(info);
    }
    public void RemoveDot(DoTDamageType st)
    {
        OnDotsChanged.Invoke(_appliedDots[st]);
        _appliedDots.Remove(st);
    }
    public Dictionary<DoTDamageType, DotInfo> GetAppliedDots()
    {
        return _appliedDots;
    }

    [NonSerialized]
    public BuffChangeEvent OnBuffsChanged = new BuffChangeEvent();
    public void AddBuff(BuffInfo info)
    {
        if (_appliedBuffs.ContainsKey(info.AffectedStat))
        {
            _appliedBuffs[info.AffectedStat].AddBuffInfo(info);
            info = _appliedBuffs[info.AffectedStat];
        }
        else
            _appliedBuffs.Add(info.AffectedStat, info);
        OnBuffsChanged.Invoke(info);
    }
    public void RemoveBuff(SubStat st)
    {
        OnBuffsChanged.Invoke(_appliedBuffs[st]);
        _appliedBuffs.Remove(st);
    }
    public Dictionary<SubStat, BuffInfo> GetAppliedBuffs()
    {
        return _appliedBuffs;
    }

    [NonSerialized]
    public StatusChangeEvent OnStatusChanged = new StatusChangeEvent();
    public void AddStatus(StatusInfo info)
    {
        if (_appliedStatuses.ContainsKey(info.Type))
        {
            _appliedStatuses[info.Type].TurnCount += info.TurnCount;
            info = _appliedStatuses[info.Type];
        }
        else
            _appliedStatuses.Add(info.Type, info);
        OnStatusChanged.Invoke(info);
    }
    public void RemoveStatus(StatusType st)
    {
        OnStatusChanged.Invoke(_appliedStatuses[st]);
        _appliedStatuses.Remove(st);
    }
    public Dictionary<StatusType, StatusInfo> GetAppliedStatuses()
    {
        return _appliedStatuses;
    }

    [NonSerialized]
    public MarkerChangeEvent OnMarkerChanged = new MarkerChangeEvent();
    public void AddMarker(MarkerInfo info, bool stackable)
    {
        if (_appliedMarkers.ContainsKey(info.Type) && stackable)
        {
            _appliedMarkers[info.Type].TurnCount += info.TurnCount;
            info = _appliedMarkers[info.Type];
        }
        else
        {
            _appliedMarkers.Add(info.Type, info);
        }
        OnMarkerChanged.Invoke(info);
    }
    public void RemoveMarker(MarkerType st)
    {
        OnMarkerChanged.Invoke(_appliedMarkers[st]);
        _appliedMarkers.Remove(st);
    }
    public Dictionary<MarkerType, MarkerInfo> GetAppliedMarkers()
    {
        return _appliedMarkers;
    }
    [NonSerialized]
    public LifeChangeEvent OnLifeChanged = new LifeChangeEvent();


    public int ApplyDamage(Character caster, DamageType type, float amount, float ignoreResistance)
    {
        if (_appliedMarkers.TryGetValue(MarkerType.IncreasedDamage, out var marker))
        {
            var damageMarker = (DamageMarkerInfo)marker;
            var casterEqualsMarker = caster.id == damageMarker.Caster.id;

            if (damageMarker.InfluencedBy != InfluenceSource.AllButCaster && casterEqualsMarker
            || damageMarker.InfluencedBy != InfluenceSource.Caster && !casterEqualsMarker)
            {
                amount *= damageMarker.DamageChange;
            }
        }

        switch (type)
        {
            case DamageType.Physical:
                amount = (int)(amount - amount * Mathf.Max(Sub.Armor - ignoreResistance, 0));
                break;
            case DamageType.Fire:
                amount = (int)(amount - amount * Mathf.Max(Sub.FireResistance - ignoreResistance, 0));
                break;
            case DamageType.Cold:
                amount = (int)(amount - amount * Mathf.Max(Sub.IceResistance - ignoreResistance, 0));
                break;
            case DamageType.Lightning:
                amount = (int)(amount - amount * Mathf.Max(Sub.LightningResistance - ignoreResistance, 0));
                break;
        }
        CurrentLife -= (int)amount;
        return (int)amount;
    }

    public int ApplyHeal(int amount)
    {
        if (_appliedMarkers.ContainsKey(MarkerType.IncreasedHealing))
        {
            amount = (int)(amount * 1.2f);
        }
        var oldLife = CurrentLife;
        CurrentLife = Mathf.Min(CurrentLife + amount, MaxLife);
        return oldLife - CurrentLife;
    }

    public void ChangeTurnCount(AffectedTypes type, int change)
    {
        switch (type)
        {
            case AffectedTypes.Buffs:
                foreach (var buff in _appliedBuffs)
                {
                    buff.Value.TurnCount += change;
                    if (buff.Value.TurnCount <= 0)
                    {
                        _appliedBuffs.Remove(buff.Key);
                    }
                    OnBuffsChanged?.Invoke(buff.Value);
                }
                break;
            case AffectedTypes.Dots:
                foreach (var dot in _appliedDots)
                {
                    dot.Value.TurnCount += change;
                    if (dot.Value.TurnCount <= 0)
                    {
                        _appliedDots.Remove(dot.Key);
                    }
                    OnDotsChanged?.Invoke(dot.Value);
                }
                break;
            case AffectedTypes.Markers:
                foreach (var marker in _appliedMarkers)
                {
                    marker.Value.TurnCount += change;
                    if (marker.Value.TurnCount <= 0)
                    {
                        _appliedMarkers.Remove(marker.Key);
                    }
                    OnMarkerChanged?.Invoke(marker.Value);
                }
                break;
            case AffectedTypes.Status:
                foreach (var status in _appliedStatuses)
                {
                    status.Value.TurnCount += change;
                    if (status.Value.TurnCount <= 0)
                    {
                        _appliedStatuses.Remove(status.Key);
                    }
                    OnStatusChanged?.Invoke(status.Value);
                }
                break;
        }
    }

#if UNITY_EDITOR
    public void AddSkill(Skill newSkill)
    {
        if (coreSkills == null)
        {
            coreSkills = new Skill[] { newSkill };
        }
        else
        {
            coreSkills = new List<Skill>(coreSkills) { newSkill }.ToArray();
        }

        Debug.Log("Here");

        EditorUtility.SetDirty(this);
    }

    public void RemoveSkill(Skill removedSkill)
    {
        var list = new List<Skill>(coreSkills);
        list.Remove(removedSkill);
        coreSkills = list.ToArray();
        EditorUtility.SetDirty(this);
    }
#endif

    public void ApplyRegen()
    {
        CurrentLife += Sub.HealthRegen;
        CurrentMana += Sub.ManaRegen;
    }

    public void ReduceTurnCounts()
    {
        var removeDot = new List<DoTDamageType>();
        foreach (var dot in _appliedDots.ToList())
        {
            var dotInfo = dot.Value;
            ApplyDamage(this, dotInfo.DamageType, dotInfo.FlatDamage + dotInfo.PercentageDamage * MaxLife, 0);
            dot.Value.TurnCount -= 1;

            if (dot.Value.TurnCount <= 0)
            {
                removeDot.Add(dot.Key);
            }
            OnDotsChanged?.Invoke(dot.Value);
        }
        removeDot.ForEach(dot => _appliedDots.Remove(dot));

        var removeStatus = new List<StatusType>();
        foreach (var status in _appliedStatuses.ToList())
        {
            status.Value.TurnCount -= 1;

            if (status.Value.TurnCount <= 0)
            {
                removeStatus.Add(status.Key);
            }
            OnStatusChanged?.Invoke(status.Value);
        }
        removeStatus.ForEach(status => _appliedStatuses.Remove(status));

        var removeBuff = new List<SubStat>();
        foreach (var buff in _appliedBuffs.ToList())
        {
            buff.Value.TurnCount -= 1;

            if (buff.Value.TurnCount <= 0)
            {
                removeBuff.Add(buff.Key);
            }
            OnBuffsChanged?.Invoke(buff.Value);
        }
        removeBuff.ForEach(buff => _appliedBuffs.Remove(buff));


        var removeMarker = new List<MarkerType>();
        foreach (var marker in _appliedMarkers.ToList())
        {
            marker.Value.TurnCount -= 1;

            if (marker.Value.TurnCount <= 0)
            {
                removeMarker.Add(marker.Key);
            }
            OnMarkerChanged?.Invoke(marker.Value);
        }
        removeMarker.ForEach(marker => _appliedMarkers.Remove(marker));
    }
}

[System.Serializable]
public class Hero : Character
{
    public Level level;
    public Rarity rarity;
    public Race race;
    public FightClass fightClass;
    public ItemData[] items;
    public SkillInfo[] skillInfos;

    int minPhysDamage;
    int maxPhysDamage;

    public int MinPhysicalDamage => Mathf.Max((int)(minPhysDamage + minPhysDamage * Sub.PhysicalDamage / 100f), 1);
    public int MaxPhysicalDamage => Mathf.Max((int)(maxPhysDamage + maxPhysDamage * Sub.PhysicalDamage / 100f), 1);

    public int AverageDamage => CalculateAverageDamage();
    public int AverageToughness => CalculateAverageToughness();
    public int AverageUtility => CalculateAverageUtility();

    [NonSerialized]
    public Dictionary<ItemType, Item> appliedItems;
    [NonSerialized]
    static readonly Dictionary<MainStat, ItemSubStatValue[]> mainToSubConversion = new Dictionary<MainStat, ItemSubStatValue[]> {
        {MainStat.Strength, new[]{   
            new ItemSubStatValue(SubStat.Health, 1),
            new ItemSubStatValue(SubStat.PhysicalDamage, 4),
            new ItemSubStatValue(SubStat.Weight, 3), 
            new ItemSubStatValue(SubStat.HealthRegen, 1), 
            }
        },
        {MainStat.Constitution, new[]{
            new ItemSubStatValue(SubStat.Health, 2),
            new ItemSubStatValue(SubStat.Armor, 1),
            new ItemSubStatValue(SubStat.FireResistance, 1),
            new ItemSubStatValue(SubStat.IceResistance, 1),
            new ItemSubStatValue(SubStat.LightningResistance, 1),
            new ItemSubStatValue(SubStat.Weight, 2), 
            new ItemSubStatValue(SubStat.HealthRegen, 2) 
            } 
        },
        {MainStat.Dexterity, new[]{
            new ItemSubStatValue(SubStat.Dodge, 3), 
            new ItemSubStatValue(SubStat.Speed, 4), 
            new ItemSubStatValue(SubStat.PhysicalDamage, 2), 
            new ItemSubStatValue(SubStat.CriticalChance, 2) 
            } 
        },
        {MainStat.Intelligence, new[]{
            new ItemSubStatValue(SubStat.Mana, 1), 
            new ItemSubStatValue(SubStat.ManaRegen, 1), 
            new ItemSubStatValue(SubStat.MagicalDamage, 5), 
            new ItemSubStatValue(SubStat.FireResistance, 1), 
            new ItemSubStatValue(SubStat.IceResistance, 1), 
            new ItemSubStatValue(SubStat.LightningResistance, 1) 
            } 
        },
        {MainStat.Luck, new[]{ 
            new ItemSubStatValue(SubStat.CriticalChance, 6),
            new ItemSubStatValue(SubStat.CriticalMagic, 2), 
            new ItemSubStatValue(SubStat.CriticalDamage, 2)
            } 
        }
    };

    public void AddCoreStat(int points, float[] dist, int life, int mana)
    {
        coreMain.str += (int)(points * dist[0]);
        coreMain.con += (int)(points * dist[1]);
        coreMain.dex += (int)(points * dist[2]);
        coreMain.intel += (int)(points * dist[3]);
        coreMain.lck += (int)(points * dist[4]);

        coreMaxLife += life;
        CurrentLife = MaxLife;
        coreMaxMana += mana;
        CurrentMana = MaxMana;
    }

    public void ApplySkillInfos()
    {
        var skillList = new List<Skill>(skillInfos.Length);
        for (var i = 0; i < skillInfos.Length; i++)
        {
            skillList.Add(DataHolder._data.SkillsManager.GetSkill(fightClass, skillInfos[i].id));
        }
        coreSkills = skillList.ToArray();
    }

    public void ApplyItems()
    {
        itemsMain = new MainStats();
        itemsSub = new SubStats();
        appliedItems = new Dictionary<ItemType, Item>();
        if (items == null) return;
        foreach (var itemData in items)
        {
            var item = ScriptableObject.CreateInstance<Item>();
            item.SetData(itemData);
            appliedItems.Add(item.type, item);
            ApplyItemStats(item);
        }

        if (appliedItems.TryGetValue(ItemType.Weapon, out Item weapon))
        {
            var baseItem = weapon.Base as Weapon;
            if (baseItem == null) return;
            var levelMult = weapon.level / 5;

            minPhysDamage = baseItem.MinDamage + levelMult * baseItem.MinDamageIncrease;
            maxPhysDamage = baseItem.MaxDamage + levelMult * baseItem.MaxDamageIncrease;
        }

        CombineMainToSub();
    }

    void ApplyItemStats(Item item)
    {
        foreach (var statChange in item.main)
        {
            itemsMain = itemsMain.AddMainStatChange(statChange);
        }

        foreach (var statChange in item.sub)
        {
            itemsSub = itemsSub.AddSubStatChange(statChange);
        }
    }

    void CombineMainToSub()
    {
        combinedSub = new SubStats();
        AddSubStats(Main.str, MainStat.Strength);
        AddSubStats(Main.con, MainStat.Constitution);
        AddSubStats(Main.dex, MainStat.Dexterity);
        AddSubStats(Main.intel, MainStat.Intelligence);
        AddSubStats(Main.lck, MainStat.Luck);
    }

    void AddSubStats(int value, MainStat mainStat)
    {
        if (value <= 0) return;
        var stats = mainToSubConversion[mainStat];
        var newStats = stats.Scale(value);
        foreach (var statChange in newStats)
        {
            combinedSub = combinedSub.AddSubStatChange(statChange);
        }
    }

    public Item AddItem(Item item)
    {
        var itemList = items != null ? items.ToList() : new List<ItemData>();
        Item oldItem = null;
        if (appliedItems != null && appliedItems.TryGetValue(item.type, out oldItem))
        {
            itemList.Remove(oldItem.GetItemData());
        }

        itemList.Add(item.GetItemData());
        items = itemList.ToArray();
        
        ApplyItems();
        return oldItem;
    }

    public void RemoveItem(Item item)
    {
        var itemList = items.ToList();
        itemList.Remove(item.GetItemData());
        items = itemList.ToArray();
        ApplyItems();
    }

    public int GetLevel()
    {
        return level.currentLevel;
    }

    public Sprite GetPortrait()
    {
        return DataHolder._data.raceImages[(int)race];
    }

    public void AddExperience(int value)
    {
        level.AddExperience(value);
    }

    public int GetHeroStrength()
    {
        return (Main.con + Main.dex + Main.intel + Main.lck + Main.str) * 5 +
            (Sub.armor + Sub.critChance + Sub.critDmg +
            Sub.critMagic + Sub.dodge + Sub.fireRes +
            Sub.hp + Sub.iceRes + Sub.lightRes +
            Sub.magicDmg + Sub.physDmg + Sub.speed + Sub.weight) * 2;
    }

    public Dictionary<ItemType, Item> GetAppliedItems()
    {
        return appliedItems;
    }

    public Tuple<int, int, int> GetAverages(Item item)
    {
        var copyChar = Instantiate(this);
        var itemList = copyChar.items != null ? copyChar.items.ToList() : new List<ItemData>();
        copyChar.ApplyItems();

        if (copyChar.appliedItems != null && copyChar.appliedItems.ContainsKey(item.type))
        {
            var oldItem = copyChar.appliedItems[item.type];
            itemList.Remove(oldItem.GetItemData());
        }

        itemList.Add(item.GetItemData());
        copyChar.items = itemList.ToArray();
        copyChar.ApplyItems();

        return new Tuple<int, int, int>(copyChar.CalculateAverageDamage(), copyChar.CalculateAverageToughness(), copyChar.CalculateAverageUtility());
    }

    int CalculateAverageDamage()
    {
        var averageDmg = ((MinPhysicalDamage + MaxPhysicalDamage) / 2f);
        var critChance = Sub.CriticalChance;
        var critMulti = 1 + Sub.CriticalDamage;
        return (int)(averageDmg * (1f - critChance) + averageDmg * critChance * critMulti);
    }

    int CalculateAverageToughness()
    {
        var averageRes = (Sub.Armor * 0.4f + Sub.FireResistance * 0.2f + Sub.IceResistance * 0.2f + Sub.LightningResistance * 0.2f);
        var dodge = Sub.Dodge;
        return (int)(MaxLife / ((1f - averageRes) * (1f - dodge)));
    }

    int CalculateAverageUtility()
    {
        return (int)(Sub.Speed + Sub.Weight);
    }
}

[System.Serializable]
public class MainStats
{
    public int str = 0;
    public int con = 0;
    public int dex = 0;
    public int intel = 0;
    public int lck = 0;

    public int GetValue(MainStat stat)
    {
        switch (stat)
        {
            case MainStat.Strength: return str;
            case MainStat.Constitution: return con;
            case MainStat.Dexterity: return dex;
            case MainStat.Intelligence: return intel;
            case MainStat.Luck: return lck;
        }
        return 0;
    }
}

[System.Serializable]
public class SubStats
{
    public int hp = 0;
    public int armor = 0;
    public int dodge = 0;
    public int speed = 0;
    public int physDmg = 0;
    public int critDmg = 0;
    public int magicDmg = 0;
    public int critMagic = 0;
    public int critChance = 0;
    public int fireRes = 0;
    public int iceRes = 0;
    public int lightRes = 0;
    public int weight = 0;
    public int mana = 0;

    public int manaRegen = 0;
    public int healthRegen = 0;

    const float HealthConversion = 100.0f;
    const float ManaConversion = 50.0f;
    const float HealthRegenConversion = 1000.0f;
    const float ManaRegenConversion = 50.0f;
    const float ArmorConversion = 100000.0f;
    const float DodgeConversion = 1000.0f;
    const float SpeedConversion = 100.0f;
    const float PhysDmgConversion = 5000.0f;
    const float CritDmgConversion = 1000.0f;
    const float MagicDmgConversion = 5000.0f;
    const float CritMagicConversion = 10000.0f;
    const float CritChanceConversion = 1000.0f;
    const float FireResConversion = 5000.0f;
    const float IceResConversion = 5000.0f;
    const float LightResConversion = 5000.0f;
    const float WeightConversion = 300.0f;

    public int Health => Mathf.CeilToInt(ConvertBySubStat(SubStat.Health, hp));
    public int Mana => Mathf.CeilToInt(ConvertBySubStat(SubStat.Mana, mana));
    public int HealthRegen => Mathf.CeilToInt(ConvertBySubStat(SubStat.HealthRegen, healthRegen));
    public int ManaRegen => Mathf.CeilToInt(ConvertBySubStat(SubStat.ManaRegen, manaRegen));

    public float Armor => ConvertBySubStat(SubStat.Armor, armor);
    public float Dodge => ConvertBySubStat(SubStat.Dodge, dodge);
    public float Speed => ConvertBySubStat(SubStat.Speed, speed);
    public float PhysicalDamage => ConvertBySubStat(SubStat.PhysicalDamage, physDmg);
    public float CriticalDamage => ConvertBySubStat(SubStat.CriticalDamage, critDmg);
    public float MagicDamage => ConvertBySubStat(SubStat.MagicalDamage, magicDmg);
    public float MagicCritical => ConvertBySubStat(SubStat.CriticalMagic, critMagic);
    public float CriticalChance => ConvertBySubStat(SubStat.CriticalChance, critChance);
    public float FireResistance => ConvertBySubStat(SubStat.FireResistance, fireRes);
    public float IceResistance => ConvertBySubStat(SubStat.IceResistance, iceRes);
    public float LightningResistance => ConvertBySubStat(SubStat.LightningResistance, lightRes);
    public int Weight => Mathf.CeilToInt(ConvertBySubStat(SubStat.Weight, weight));

    public static float ConvertBySubStat(SubStat stat, int value)
    {
        switch (stat)
        {
            case SubStat.Health:
                return value / HealthConversion;
            case SubStat.Mana:
                return value / ManaConversion;
            case SubStat.HealthRegen:
                return value / HealthRegenConversion;
            case SubStat.ManaRegen:
                return value / ManaRegenConversion;
            case SubStat.Armor:
                return value / ArmorConversion;
            case SubStat.Dodge:
                return value / DodgeConversion;
            case SubStat.Speed:
                return value / SpeedConversion;
            case SubStat.PhysicalDamage:
                return value / PhysDmgConversion;
            case SubStat.CriticalDamage:
                return value / CritDmgConversion;
            case SubStat.MagicalDamage:
                return value / MagicDmgConversion;
            case SubStat.CriticalMagic:
                return value / CritMagicConversion;
            case SubStat.CriticalChance:
                return value / CritChanceConversion;
            case SubStat.FireResistance:
                return value / FireResConversion;
            case SubStat.IceResistance:
                return value / IceResConversion;
            case SubStat.LightningResistance:
                return value / LightResConversion;
            case SubStat.Weight:
                return value / WeightConversion;
        }
        return value;
    }

    public static string GetSimpleString(SubStat stat, int value)
    {

        return $"{ConvertBySubStat(stat, value):N0}{GetEndingBySubStat(stat)}";
    }

    static string GetEndingBySubStat(SubStat stat)
    {
        switch (stat)
        {
            case SubStat.Mana:
            case SubStat.Health:
            case SubStat.Armor:
            case SubStat.Speed:
            case SubStat.HealthRegen:
            case SubStat.ManaRegen:
                return string.Empty;
            case SubStat.Dodge:
            case SubStat.PhysicalDamage:
            case SubStat.CriticalDamage:
            case SubStat.MagicalDamage:
            case SubStat.CriticalMagic:
            case SubStat.CriticalChance:
            case SubStat.FireResistance:
            case SubStat.IceResistance:
            case SubStat.LightningResistance:
            case SubStat.Weight:
                return "%";
            case SubStat.None:
                break;
        }
        return string.Empty;
    }
}

public enum MainStat
{
    Strength,
    Constitution,
    Dexterity,
    Intelligence,
    Luck,
    None = 99
}

public enum SubStat
{
    Health,
    Armor,
    Dodge,
    Speed,
    PhysicalDamage,
    CriticalDamage,
    MagicalDamage,
    CriticalMagic,
    CriticalChance,
    FireResistance,
    IceResistance,
    LightningResistance,
    Weight,
    Mana,
    HealthRegen,
    ManaRegen,
    None = 99
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
    Barbarian,
    Paladin,
    Gladiator,
    Cleric,
    Monk,
    Templar,
    Ranger,
    Druid,
    Assassin,
    Wizard,
    Warlock,
    Guardian,
    Rogue,
    Slayer,
    Acolyth
}

[Serializable]
public struct Level
{
    public int currentLevel;
    public int currentExp;

    public bool CanBeLeveled => currentExp >= CharactersService.ExperienceByLevel[currentLevel];

    public Level(int level, int exp)
    {
        currentLevel = level;
        currentExp = exp;
    }

    public void AddExperience(int addedExp)
    {
        currentExp += addedExp;
    }

    public void LevelUp()
    {
        currentExp = 0;
        currentLevel++;
    }
}

[Serializable]
public struct SkillInfo
{
    public Level level;
    public int id;

    public SkillInfo(Level newLevel, int newId)
    {
        level = newLevel;
        id = newId;
    }

    public void AddExperience(int addedExp)
    {
        level.AddExperience(addedExp);
    }

    public void LevelUp()
    {
        level.LevelUp();
    }
}
