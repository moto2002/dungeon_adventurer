using System.Collections.Generic;
using UnityEngine;

public class DungeonController : Controller
{
    Dungeon _appliedDungeon;

    struct BattleType
    {
        public BattleTypes type;
        public int minMonsters;
        public int maxMonsters;
    }

    readonly BattleType NORMAL_BATTLE = new BattleType { type = BattleTypes.Normal, maxMonsters = 3, minMonsters = 1 };
    readonly BattleType ELITE_BATTLE = new BattleType { type = BattleTypes.Elite, maxMonsters = 4, minMonsters = 2 };
    readonly BattleType CHAMPION_BATTLE = new BattleType { type = BattleTypes.Champion, maxMonsters = 4, minMonsters = 3 };
    readonly BattleType BOSS_BATTLE = new BattleType { type = BattleTypes.Boss, maxMonsters = 2, minMonsters = 1 };

    const float NORMAL_CHANCE = 0.55f;
    const float ELITE_CHANCE = 0.35f;
    const float CHAMPION_CHANCE = 0.1f;

    public DungeonController(Dungeon dungeon)
    {
        _appliedDungeon = dungeon;
    }

    public void Init()
    {
        Debug.Log("Min" + _appliedDungeon.minLvl);
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
        var posMonsters = _appliedDungeon.possibleMonsters;

        if (monsterCount >= posMonsters.Length)
        {
            foreach (var mon in posMonsters)
            {
                monsters.Add(new Monster(mon));
            }
        }

        for (int i = 0; i < monsterCount - monsters.Count; i++)
        {
            var rands = Random.Range(0, posMonsters.Length);
            monsters.Add(new Monster(posMonsters[rands]));
        }

        SceneManagement._manager.Show("View_Battle", new BattleController(monsters.ToArray()));
    }
    
    public enum BattleTypes
    {
        Normal,
        Elite,
        Champion,
        Boss
    }
}
