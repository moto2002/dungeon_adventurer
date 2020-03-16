using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Custom/Dungeon")]
public class Dungeon : ScriptableObject
{
    public int id;
    public string displayName;
    public int minLvl;
    public int maxLvl;

    public Monster[] possibleMonsters;
    public Monster boss;

#if UNITY_EDITOR
    public void AddPosMonster(Monster newMonster)
    {
        possibleMonsters = new List<Monster>(possibleMonsters) { newMonster }.ToArray();
        EditorUtility.SetDirty(this);
    }

    public void RemovePosMonster(Monster removedMonster)
    {
        var monsterList = new List<Monster>(possibleMonsters);
        monsterList.Remove(removedMonster);
        possibleMonsters = monsterList.ToArray();
        EditorUtility.SetDirty(this);
    }
#endif
}

