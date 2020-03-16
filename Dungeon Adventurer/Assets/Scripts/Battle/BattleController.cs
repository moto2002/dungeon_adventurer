using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : Controller
{
    public List<Monster> Monsters { get; private set; }
    public List<Hero> Heroes { get; private set; }
    public Dictionary<int, Monster> positionedMonsters { get; private set; }
    public Dictionary<int, Character> allCharacters { get; private set; }

    public UnityEvent battleEnded = new UnityEvent();

    int _level;
    int _aliveEnemies = 0;
    BattleResult _battleResult = new BattleResult();

    public BattleController(List<Monster> monsters, Hero[] heroes, int dungeonLevel)
    {
        _level = dungeonLevel;

        allCharacters = new Dictionary<int, Character>();

        Monsters = monsters;

        for (int i = 0; i < Monsters.Count; i++)
        {
            Monsters[i].id = (i + 1) * -1;
        }
        
        Heroes = heroes.ToList().FindAll(e => e.position <= 9);

        foreach (var hero in this.Heroes)
        {
            allCharacters.Add(hero.id, hero);
            hero.OnCharacterDeath.AddListener(OnHeroDeath);
            hero.Skills.ToList().ForEach(skill => skill.SetCooldown(0));
        }
        foreach (var monster in this.Monsters)
        {
            allCharacters.Add(monster.id, monster);
            monster.OnCharacterDeath.AddListener(OnMonsterDeath);
        }

        SetPositions();
        CheckMonsterDict();
    }

    void OnHeroDeath(int id)
    {
        Heroes.Remove(Heroes.Find(e => e.id == id));
        allCharacters.Remove(id);
    }

    void OnMonsterDeath(int id)
    {
        Debug.Log($"Monster with id {id} died, current Monsters is {Monsters.Count}");
        var monster = Monsters.First(e => e.id == id);
        _battleResult.Experience += monster.GetExperience();
        _battleResult.Gold += Random.Range(50, 500) * _level;
        _battleResult.Items.Add( ItemCreator.CreateItem(monster.level));
        Monsters.Remove(monster);
        positionedMonsters.Remove(positionedMonsters.FirstOrDefault(e => e.Value.id == id).Key);
        allCharacters.Remove(id);
        CheckMonsterDict();
    }

    void CheckMonsterDict()
    {
        _aliveEnemies = positionedMonsters.Count;
        if (_aliveEnemies <= 0)
        {
            Endbattle();
        }
    }

    void Endbattle()
    {
        battleEnded?.Invoke();
        Win();
    }

    public void Retreat()
    {
        battleEnded?.Invoke();
        _battleResult.Clean();
        _battleResult.Victorious = false;
        ViewUtility.ShowThenHide<DungeonView, BattleView>().Then(view=> view.SetBattleResult(_battleResult)).Done();
    }

    public void Win()
    {
        _battleResult.Victorious = true;
        ViewUtility.ShowThenHide<DungeonView, BattleView>().Then(view => view.SetBattleResult(_battleResult)).Done();
    }

    void SetPositions()
    {
        positionedMonsters = new Dictionary<int, Monster>(); ;
        foreach (var mons in Monsters)
        {
            var wantedPos = mons.GetPossiblePositions();
            while (true)
            {
                var pos = wantedPos[Random.Range(0, wantedPos.Count)];
                if (!positionedMonsters.ContainsKey(pos))
                {
                    positionedMonsters.Add(pos, mons);
                    mons.position = pos;
                    break;
                }
                wantedPos.Remove(pos);
            }
        }
    }
}

public class BattleResult
{
    public bool Victorious;
    public float Experience;
    public double Gold;
    public List<ItemData> Items = new List<ItemData>();

    public void Clean()
    {
        Experience = 0;
        Gold = 0;
        Items.Clear();
    }
}
