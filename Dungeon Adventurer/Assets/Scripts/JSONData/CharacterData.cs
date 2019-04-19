using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CharacterData {
    public Hero[] characters;
}
public class CharDeathEvent : UnityEvent<int> { }
public class LifeChangeEvent : UnityEvent<int> { }

public class Character : ScriptableObject {
    public int id;
    public string name;
    public int position;
    public MainStats main;
    public SubStats sub;
    public Skill[] skills;
    Dictionary<StatusType, int> _appliedStatuses = new Dictionary<StatusType, int>();
    Dictionary<MarkerType, int> _appliedMarkers = new Dictionary<MarkerType, int>();
    public int maxLife;
    public int currentLife = 0;

    [NonSerialized]
    public CharDeathEvent OnCharacterDeath = new CharDeathEvent();

    [NonSerialized]
    public UnityEvent OnStatusChanged = new UnityEvent();
    public void AddStatus(StatusType st, int i) {
        if (_appliedStatuses.ContainsKey(st))
            _appliedStatuses[st] += i;
        else
            _appliedStatuses.Add(st, i);
        OnStatusChanged.Invoke();
    }
    public void RemoveStatus(StatusType st) {
        _appliedStatuses.Remove(st);
        OnStatusChanged.Invoke();
    }
    public Dictionary<StatusType, int> GetAppliedStatuses() {
        return _appliedStatuses;
    }

    [NonSerialized]
    public UnityEvent OnMarkerChanged = new UnityEvent();
    public void AddMarker(MarkerType st, int i) {
        if (_appliedMarkers.ContainsKey(st))
            _appliedMarkers[st] += i;
        else
            _appliedMarkers.Add(st, i);
        OnMarkerChanged.Invoke();
    }
    public void RemoveMarker(MarkerType st) {
        _appliedMarkers.Remove(st);
        OnMarkerChanged.Invoke();
    }
    public Dictionary<MarkerType, int> GetAppliedMarkers() {
        return _appliedMarkers;
    }
    [NonSerialized]
    public LifeChangeEvent OnLifeChanged = new LifeChangeEvent();
    public int CurrentLife {
        get
        {
            return currentLife;
        }
        set
        {
            var oldvalue = currentLife;
            currentLife = value;
            OnLifeChanged.Invoke(currentLife - oldvalue);
            if (currentLife <= 0)
                OnCharacterDeath.Invoke(id);
        }
    }
}

[System.Serializable]
public class Hero : Character {
    public int experience;
    public Rarity rarity;
    public Race race;
    public FightClass fightClass;

    public ItemData[] items;

    public Dictionary<ItemType, Item> appliedItems;

    public void ApplyItems() {
        foreach (var itemData in items) {
            var item = DataHolder._data.ItemDict[itemData.id];
            if (itemData.mainStatChanges.Length != 0) item.ApplyMainChanges(itemData.mainStatChanges);

            appliedItems.Add(item.type, item);
        }
    }
}

[System.Serializable]
public class MainStats {
    public int str;
    public int con;
    public int dex;
    public int intel;
    public int lck;
}

[System.Serializable]
public class SubStats {
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

public enum MainStat {
    Strength,
    Constitution,
    Dexterity,
    Intelligence,
    Luck,
}

public enum SubStat {
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
}

public enum Race {
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

public enum Rarity {
    Common,
    Uncommon,
    Magic,
    Rare,
    Epic,
    Legendary,
    Unique
}

public enum FightClass {
    Warrior,
    Tank,
    Ranger,
    Priest,
    Guardian
}
