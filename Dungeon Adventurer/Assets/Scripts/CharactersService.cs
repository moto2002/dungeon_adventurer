using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharactersService : EventPublisher
{
    public struct CharacterLevelStat
    {
        public Tuple<int, int> Life;
        public Tuple<int, int> Mana;
        public float[] StatDistribution;
    }

    const int LevelNum = 100;
    const int BaseExp = 4;
    const double ExpChange = 0.03039118551;
    const int SkillLevelNum = 20;
    const int SkillBaseExp = 20;
    const double SkillExpChange = 0.2766363459;

    public static readonly Dictionary<int, int> SkillExperienceByLevel = new Dictionary<int, int>();
    public static readonly Dictionary<int, int> ExperienceByLevel = new Dictionary<int, int>();
    public static readonly Dictionary<FightClass, CharacterLevelStat> LevelStats = new Dictionary<FightClass, CharacterLevelStat>()
    {
        {FightClass.Acolyth, new CharacterLevelStat(){
            Life = new Tuple<int, int> (9, 19), Mana = new Tuple<int, int> (7, 15), StatDistribution = new []{ 0.1f, 0.3f, 0.1f, 0.2f, 0.3f }}},
        {FightClass.Assassin, new CharacterLevelStat(){
            Life = new Tuple<int, int> (6, 13), Mana = new Tuple<int, int> (6, 13), StatDistribution = new []{ 0.1f, 0.3f, 0.2f, 0.1f, 0.3f }}},
        {FightClass.Barbarian, new CharacterLevelStat(){
            Life = new Tuple<int, int> (10, 21), Mana = new Tuple<int, int> (2, 5), StatDistribution = new []{ 0.4f, 0.2f, 0.2f, 0.1f, 0.1f }}},
        {FightClass.Cleric, new CharacterLevelStat()
        { Life = new Tuple<int, int> (12, 25), Mana = new Tuple<int, int> (6, 13), StatDistribution = new []{ 0.2f, 0.4f, 0.1f, 0.2f, 0.1f }}},
        {FightClass.Druid, new CharacterLevelStat(){
            Life = new Tuple<int, int> (6, 13), Mana = new Tuple<int, int> (7, 15), StatDistribution = new []{ 0.1f, 0.2f, 0.3f, 0.3f, 0.1f }}},
        {FightClass.Gladiator, new CharacterLevelStat(){
            Life = new Tuple<int, int> (9, 19), Mana = new Tuple<int, int> (3, 7), StatDistribution = new []{ 0.3f, 0.2f, 0.3f, 0.1f, 0.1f }}},
        {FightClass.Guardian, new CharacterLevelStat(){
            Life = new Tuple<int, int> (7, 15), Mana = new Tuple<int, int> (6, 13), StatDistribution = new []{ 0.3f, 0.2f, 0.1f, 0.3f, 0.1f }}},
        {FightClass.Monk, new CharacterLevelStat(){
            Life = new Tuple<int, int> (10, 21), Mana = new Tuple<int, int> (5, 11), StatDistribution = new []{ 0.1f, 0.3f, 0.3f, 0.2f, 0.1f }}},
        {FightClass.Paladin, new CharacterLevelStat(){
            Life = new Tuple<int, int> (11, 23), Mana = new Tuple<int, int> (3, 7), StatDistribution = new []{ 0.3f, 0.3f, 0.2f, 0.1f, 0.1f }}},
        {FightClass.Ranger, new CharacterLevelStat(){
            Life = new Tuple<int, int> (8, 17), Mana = new Tuple<int, int> (4, 9), StatDistribution = new []{ 0.1f, 0.2f, 0.4f, 0.1f, 0.2f }}},
        {FightClass.Rogue, new CharacterLevelStat(){
            Life = new Tuple<int, int> (6, 13), Mana = new Tuple<int, int> (8, 17), StatDistribution = new []{ 0.1f, 0.2f, 0.2f, 0.1f, 0.4f }}},
        {FightClass.Slayer, new CharacterLevelStat(){
            Life = new Tuple<int, int> (8, 17), Mana = new Tuple<int, int> (5, 11), StatDistribution = new []{ 0.3f, 0.2f, 0.1f, 0.1f, 0.3f }}},
        {FightClass.Templar, new CharacterLevelStat(){
            Life = new Tuple<int, int> (8, 17), Mana = new Tuple<int, int> (8, 17), StatDistribution = new []{ 0.1f, 0.3f, 0.2f, 0.3f, 0.1f }}},
        {FightClass.Warlock, new CharacterLevelStat(){
            Life = new Tuple<int, int> (5, 11), Mana = new Tuple<int, int> (9, 19), StatDistribution = new []{ 0.1f, 0.2f, 0.1f, 0.3f, 0.3f }}},
        {FightClass.Wizard, new CharacterLevelStat(){
            Life = new Tuple<int, int> (4, 9), Mana = new Tuple<int, int> (10, 21), StatDistribution = new []{ 0.1f, 0.2f, 0.1f, 0.4f, 0.2f }}},
    };

    CharactersModel _currentHeroes;

    public interface OnCharactersChanged
    {
        void OnModelChanged(CharactersModel characters);
    }
    private delegate void OnModelChanged(CharactersModel model);
    private event OnModelChanged changedEvent;

    public override void RegisterListener(EventListener listener)
    {
        if (listener is OnCharactersChanged)
        {
            var listen = (OnCharactersChanged)listener;
            changedEvent += new OnModelChanged(listen.OnModelChanged);
            listen.OnModelChanged(_currentHeroes);
        }
    }

    public override void UnregisterListener(EventListener listener)
    {
        if (listener is OnCharactersChanged)
            changedEvent -= new OnModelChanged(((OnCharactersChanged)listener).OnModelChanged);
    }

    public CharactersService()
    {
        Init();
    }

    public override void Init()
    {
        _currentHeroes = new CharactersModel(DataHolder._data.LoadCharacters());
        ExperienceByLevel.Clear();
        for (var i = 1; i < LevelNum; i++)
        {
            var value = i;
            ExperienceByLevel.Add(i, BaseExp + value * value + (int)(Math.Pow(value, 1 + ExpChange * value)));
        }

        SkillExperienceByLevel.Clear();
        for (var i = 1; i < SkillLevelNum; i++)
        {
            var value = i;
            SkillExperienceByLevel.Add(i, SkillBaseExp + value * value + (int)(Math.Pow(value, 1 + SkillExpChange * value)));
        }
    }

    public override void Publish()
    {
        DataHolder._data.SaveCharacters(_currentHeroes.Data);
        if (changedEvent == null) return;
        changedEvent(_currentHeroes);
    }

    public void SetCharacterPosition(int charID, int position)
    {
        foreach (var characters in _currentHeroes.Characters)
        {
            if (characters.id == charID)
            {
                characters.position = position;
                break;
            }
        }

        Publish();
    }

    public void ApplyExperience(float experience)
    {
        var heroes = new List<Hero>(_currentHeroes.Characters).FindAll(e => e.position <= 9);
        var resultExp = (int)(experience / heroes.Count);
        heroes.ForEach(hero => hero.AddExperience(Mathf.Min(resultExp, ExperienceByLevel[hero.level.currentLevel] - hero.level.currentExp)));

        Publish();
    }

    public bool RankUp(int charId)
    {
        var hero = _currentHeroes.Characters.FirstOrDefault(character => character.id == charId);
        if (hero.level.currentExp < ExperienceByLevel[hero.level.currentLevel])
        {
            return false;
        }

        hero.level.LevelUp();
        var levelStats = LevelStats[hero.fightClass];
        hero.AddCoreStat(DataHolder._data.CharCreationDict[hero.rarity].levelPoints, levelStats.StatDistribution, levelStats.Life.Item2 - 1, levelStats.Mana.Item2 - 1);

        Publish();
        return true;
    }

    public Progress GetCharacterLevelProgress(int charId)
    {
        var hero = _currentHeroes.Characters.FirstOrDefault(character => character.id == charId);
        return new Progress(0, ExperienceByLevel[hero.level.currentLevel], hero.level.currentExp);
    }

    public void AddCharacter(Hero hero)
    {
        _currentHeroes.Data.AddHero(hero);
        hero.ApplySkillInfos();
        Publish();
    }

    public void RemoveCharacter(Hero hero)
    {
        _currentHeroes.Data.RemoveHero(hero);
        Publish();
    }

    public Hero[] GetHeroes()
    {
        return _currentHeroes.Characters;
    }

    public void EquipItemBackpack(Item data, Hero hero)
    {
        var oldItem = hero.AddItem(data);
        if (oldItem != null)
        {
            ServiceRegistry.Dungeon.AddItemToBackpack(oldItem.GetItemData());
        }
        ServiceRegistry.Dungeon.RemoveItemFromBackpack(data.GetItemData());
        Publish();
    }

    public void EquipItemInventory(Item data, Hero hero)
    {
        var oldItem = hero.AddItem(data);
        if(oldItem != null)
        {
            ServiceRegistry.Inventory.AddItemToInventory(oldItem.GetItemData());
        }
        ServiceRegistry.Inventory.RemoveItemFromInventory(data.GetItemData());
        Publish();
    }

    public void UnEquipItemInventory(Item data, Hero hero)
    {
        hero.RemoveItem(data);

        ServiceRegistry.Inventory.AddItemToInventory(data.GetItemData());
        Publish();
    }

    public void UnEquipItemBackpack(Item data, Hero hero)
    {
        hero.RemoveItem(data);
        ServiceRegistry.Dungeon.AddItemToBackpack(data.GetItemData());
        Publish();
    }
}
