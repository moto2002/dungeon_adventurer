using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

public class DataHolder : MonoBehaviour {
    public static DataHolder _data;

    public Lexic.NameGenerator NameGeneretor;
    [SerializeField] Item[] itemData;
    [SerializeField] Monster[] monsterData;
    [SerializeField] Dungeon[] dungeonData;
    [SerializeField] CharacterCreationData[] characterCreationData;
    [SerializeField] public Sprite[] raceImages;

    public CharacterData CharData;

    public Item[] InventoryItems;

    public Skill[] Skills;

    public Skill[] FighterDefault = new Skill[4];
    public Skill[] RangerDefault = new Skill[4];
    public Skill[] WizardDefault = new Skill[4];
    public Skill[] PriestDefault = new Skill[4];

    public Dictionary<int, Monster> MonsterDict;
    public Dictionary<int, Dungeon> DungeonDict;
    public Dictionary<int, Item> ItemDict;
    public Dictionary<Rarity, CharacterCreationData> CharCreationDict;

    private void Awake() {
        _data = this;
        DontDestroyOnLoad(gameObject);

        ItemDict = new Dictionary<int, Item>();
        foreach (var item in itemData) {
            ItemDict.Add(item.id, item);
        }
        InventoryItems = itemData;

        CharCreationDict = new Dictionary<Rarity, CharacterCreationData>();
        foreach (var charCreate in characterCreationData) {
            CharCreationDict.Add(charCreate.rarity, charCreate);
        }

        MonsterDict = new Dictionary<int, Monster>();
        foreach (var monster in monsterData) {
            MonsterDict.Add(monster.id, monster);
        }

        DungeonDict = new Dictionary<int, Dungeon>();
        foreach (var dungeon in dungeonData) {
            DungeonDict.Add(dungeon.id, dungeon);
        }

        LoadCharacters();
        foreach (var chas in CharData.characters) {

            var skills = FighterDefault;
            switch (chas.fightClass) {
                case FightClass.Warrior:
                    skills = FighterDefault;
                    break;
                case FightClass.Tank:
                    skills = FighterDefault;
                    break;
                case FightClass.Ranger:
                    skills = RangerDefault;
                    break;
                case FightClass.Priest:
                    skills = PriestDefault;
                    break;
                case FightClass.Guardian:
                    skills = WizardDefault;
                    break;
            }

            chas.skills = skills;
        }
        //SaveCharacters();
    }

    public void SetCharacterPosition(int charID, int position) {
        foreach (var characters in CharData.characters) {
            if (characters.id == charID) {
                characters.position = position;
                break;
            }
        }
        SaveCharacters();
    }

    void LoadCharacters() {
        CharData = SaveGame.Load<CharacterData>("characters");
    }

    void SaveCharacters() {
        SaveGame.Save<CharacterData>("characters", CharData);
    }
}
