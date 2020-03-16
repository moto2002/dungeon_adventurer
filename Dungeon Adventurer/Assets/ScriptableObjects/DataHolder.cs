using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public static DataHolder _data;

    public Lexic.NameGenerator NameGeneretor;
    [SerializeField] Dungeon[] dungeonData;
    [SerializeField] CharacterCreationData[] characterCreationData;
    [SerializeField] public Sprite[] raceImages;
    [SerializeField] EncounterCollection encounterCollection;

    public SkillsManager SkillsManager;

    public Dictionary<int, Dungeon> DungeonDict;
    public BaseItemManager BaseItems;
    public Dictionary<Rarity, CharacterCreationData> CharCreationDict;
    public EncounterCollection Encounters => encounterCollection;

    private void Awake()
    {
        _data = this;
        DontDestroyOnLoad(gameObject);

        CharCreationDict = new Dictionary<Rarity, CharacterCreationData>();
        foreach (var charCreate in characterCreationData)
        {
            CharCreationDict.Add(charCreate.rarity, charCreate);
        }

        DungeonDict = new Dictionary<int, Dungeon>();
        foreach (var dungeon in dungeonData)
        {
            DungeonDict.Add(dungeon.id, dungeon);
        }
    }

    public CharacterData LoadCharacters()
    {
        if (!SaveGame.Exists("characters")) return new CharacterData();
        var data = SaveGame.Load<CharacterData>("characters");
        return data;
    }

    public void SaveCharacters(CharacterData data)
    {
        SaveGame.Save<CharacterData>("characters", data);
    }

    public CurrencyData GetCurrencyData()
    {
        if (!SaveGame.Exists("currency")) return new CurrencyData();
        return SaveGame.Load<CurrencyData>("currency");
    }

    public void SaveCurrency(CurrencyData data)
    {
        SaveGame.Save<CurrencyData>("currency", data);
    }

    public ItemContainer LoadInventory()
    {
        if (!SaveGame.Exists("inventory")) return new ItemContainer();
        return SaveGame.Load<ItemContainer>("inventory");
    }

    public void SaveInventory(ItemContainer data)
    {
        SaveGame.Save<ItemContainer>("inventory", data);
    }
}
[Serializable]
public struct ItemContainer
{
    public ItemData[] items;
}
