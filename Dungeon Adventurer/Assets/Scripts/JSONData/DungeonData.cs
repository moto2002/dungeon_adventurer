using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DungeonData
{
    public Dungeon[] dungeons;
}

[System.Serializable]
public class Dungeon
{
    public string name;
    public int minLvl;
    public int maxLvl;
    public int[] possibleMonstersIds;
    public int bossId;

    public Monster[] possibleMonsters;
    public Monster boss;

    public void Instantiate()
    {
        possibleMonsters = new Monster[possibleMonstersIds.Length];
        for (int i = 0; i < possibleMonstersIds.Length; i++)
        {
            possibleMonsters[i] = Initialiser._instance._monsterData.monstersDict[possibleMonstersIds[i]];
        }

        boss = Initialiser._instance._monsterData.monstersDict[bossId];
    }

}
