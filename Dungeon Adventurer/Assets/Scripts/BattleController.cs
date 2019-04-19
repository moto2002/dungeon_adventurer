using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : Controller {
    public List<Monster> Monsters { get; private set; }
    public List<Hero> heroes { get; private set; }
    public Dictionary<int, Monster> positionedMonsters { get; private set; }
    public Dictionary<int, Character> allCharacters { get; private set; }

    public UnityEvent battleEnded = new UnityEvent();

    int _aliveEnemies = 0;

    public BattleController(Monster[] monsters) {
        allCharacters = new Dictionary<int, Character>();
        Monsters = monsters.ToList();

        for (int i = 0; i < Monsters.Count; i++) {
            Monsters[i].id = (i + 1) * -1;
        }

        this.heroes = new List<Hero>(DataHolder._data.CharData.characters).FindAll(e => e.position <= 9);

        foreach (var hero in this.heroes) {
            allCharacters.Add(hero.id, hero);
            hero.OnCharacterDeath.AddListener(RemoveFromDict);
        }
        foreach (var monster in this.Monsters) {
            allCharacters.Add(monster.id, monster);
            monster.OnCharacterDeath.AddListener(RemoveFromDict);
        }

        SetPositions();
        CheckMonsterDict();
    }

    void RemoveFromDict(int id) {
        allCharacters.Remove(id);
        if (id > 0)
            heroes.Remove(heroes.Find(e => e.id == id));
        else {
            Monsters.Remove(Monsters.Find(e => e.id == id));
            positionedMonsters.Remove(positionedMonsters.FirstOrDefault(e => e.Value.id == id).Key);
        }

        CheckMonsterDict();
    }

    void CheckMonsterDict() {
        _aliveEnemies = positionedMonsters.Count;
        if (_aliveEnemies <= 0)
            battleEnded.Invoke();
    }

    void SetPositions() {
        positionedMonsters = new Dictionary<int, Monster>(); ;
        foreach (var mons in Monsters) {
            var wantedPos = mons.GetPossiblePositions();
            while (true) {
                var pos = wantedPos[Random.Range(0, wantedPos.Count)];
                if (!positionedMonsters.ContainsKey(pos)) {
                    positionedMonsters.Add(pos, mons);
                    mons.position = pos;
                    break;
                }
                wantedPos.Remove(pos);
            }
        }
    }
}
