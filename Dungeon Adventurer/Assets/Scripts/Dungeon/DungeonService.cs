using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonService : EventPublisher
{
    const float NORMAL_CHANCE = 0.55f;
    const float ELITE_CHANCE = 0.35f;
    const float CHAMPION_CHANCE = 0.1f;

    readonly BattleType NORMAL_BATTLE = new BattleType { type = BattleTypes.Normal, maxMonsters = 3, minMonsters = 1 };
    readonly BattleType ELITE_BATTLE = new BattleType { type = BattleTypes.Elite, maxMonsters = 4, minMonsters = 2 };
    readonly BattleType CHAMPION_BATTLE = new BattleType { type = BattleTypes.Champion, maxMonsters = 4, minMonsters = 3 };
    readonly BattleType BOSS_BATTLE = new BattleType { type = BattleTypes.Boss, maxMonsters = 2, minMonsters = 1 };

    Dictionary<int, Dungeon> _dungeonDict;
    DungeonModel _currentDungeon;
    BackpackModel _currentBackpack;
    int _selectedDungeon;

    public interface OnDungeonChanged
    {
        void OnModelChanged(DungeonModel model);
    }
    private delegate void OnModelChanged(DungeonModel model);
    private event OnModelChanged changedEvent;

    public interface OnBackPackChanged
    {
        void OnModelChanged(BackpackModel model);
    }
    private delegate void OnPackChanged(BackpackModel model);
    private event OnPackChanged packChangedEvent;

    public override void RegisterListener(EventListener listener)
    {
        if (listener is OnDungeonChanged)
        {
            var listen = (OnDungeonChanged)listener;
            changedEvent += new OnModelChanged(listen.OnModelChanged);
            listen.OnModelChanged(_currentDungeon);
        }

        if (listener is OnBackPackChanged)
        {
            var listen = (OnBackPackChanged)listener;
            packChangedEvent += new OnPackChanged(listen.OnModelChanged);
            listen.OnModelChanged(_currentBackpack);
        }
    }

    public override void UnregisterListener(EventListener listener)
    {
        if (listener is OnDungeonChanged)
            changedEvent -= new OnModelChanged(((OnDungeonChanged)listener).OnModelChanged);
        if (listener is OnBackPackChanged)
            packChangedEvent -= new OnPackChanged(((OnBackPackChanged)listener).OnModelChanged);
    }

    public DungeonService()
    {
        Init();
    }

    public override void Init()
    {
        _dungeonDict = DataHolder._data.DungeonDict;
        _selectedDungeon = 1;
    }

    public override void Publish()
    {
        changedEvent?.Invoke(_currentDungeon);
    }

    public void PublishBackpack()
    {
        if (packChangedEvent != null && _currentBackpack != null) packChangedEvent(_currentBackpack);
    }

    public void SetSelectedId(int id)
    {
        _selectedDungeon = id;
    }

    public bool AddItemToBackpack(ItemData data)
    {
        if (_currentBackpack.AddItem(data))
        {
            PublishBackpack();
            return true;
        }

        return false;
    }

    public bool RemoveItemFromBackpack(ItemData data)
    {
        if (_currentBackpack.RemoveItem(data))
        {
            PublishBackpack();
            return true;
        }

        return false;
    }

    public void SellItem(ItemData data, double value)
    {
        ServiceRegistry.Currency.CreditCurrency(Currency.Coins, value);
        _currentBackpack.RemoveItem(data);
        PublishBackpack();
    }

    public void AddItemToBackpack(int charId, ItemData data)
    {
        if(_currentBackpack.AddItem(charId, data)) PublishBackpack();
    }

    public void RemoveItemFromBackpack(int charId, ItemData data)
    {
        _currentBackpack.RemoveItem(charId, data);
        PublishBackpack();
    }

    public void StartDungeon()
    {
        var heroes = ServiceRegistry.Characters.GetHeroes();
        var posSlots = new Dictionary<int, int>();

        var enteredHeroes = new List<Hero>();

        foreach (var hero in heroes)
        {
            if (hero.position < 9 && hero.position > 0)
            {
                posSlots.Add(hero.id, hero.Sub.Weight);
                enteredHeroes.Add(hero);
            }
        }
        _currentBackpack = new BackpackModel(posSlots);
        _currentDungeon = new DungeonModel(enteredHeroes, _dungeonDict[_selectedDungeon]);

        Publish();
    }

    public void StartBattle()
    {
        BattleType battle = NORMAL_BATTLE;
        var rand = Random.value;
        if (rand <= CHAMPION_CHANCE)
        {
            battle = CHAMPION_BATTLE;
        }
        else if (rand < ELITE_CHANCE + CHAMPION_CHANCE)
        {
            battle = ELITE_BATTLE;
        }

        var monsterCount = Random.Range(battle.minMonsters, battle.maxMonsters + 1);

        List<Monster> monsters = new List<Monster>();
        var posMonsters = _currentDungeon.Dungeon.possibleMonsters;
        var monsterLevel = Random.Range(_currentDungeon.Dungeon.minLvl, _currentDungeon.Dungeon.maxLvl + 1);

        if (monsterCount >= posMonsters.Length)
        {
            foreach (var mon in posMonsters)
            {
                var monster = ScriptableObject.CreateInstance<Monster>();
                monster.SetData(mon);
                monster.SetLevel(monsterLevel);
                monsters.Add(monster);
            }
        }

        for (int i = 0; i < monsterCount - monsters.Count; i++)
        {
            var rands = Random.Range(0, posMonsters.Length);
            var monster = ScriptableObject.CreateInstance<Monster>();
            monster.SetData(posMonsters[rands]);
            monster.SetLevel(monsterLevel);
            monsters.Add(monster);
        }

        ViewUtility.Show<BattleView>(view => view.SetController(new BattleController(monsters, ServiceRegistry.Characters.GetHeroes(), _currentDungeon.Dungeon.minLvl))).Done();
    }

    public void ApplyBuff(ADungeonBuff buff, Hero hero)
    {
        if(buff.GetType() == typeof(DungeonBuff))
        {
            _currentDungeon.ApplyDungeonBuff((DungeonBuff)buff);
            return;
        }

        if (buff.GetType() == typeof(BattleBuff))
        {
            _currentDungeon.ApplyBattleBuff((BattleBuff)buff, hero);
            return;
        }

        if (buff.GetType() == typeof(RoomBuff))
        {
            _currentDungeon.ApplyRoomBuff((RoomBuff)buff, hero);
            return;
        }
    }

    struct BattleType
    {
        public BattleTypes type;
        public int minMonsters;
        public int maxMonsters;
    }
}
