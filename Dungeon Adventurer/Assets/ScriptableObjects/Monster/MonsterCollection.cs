#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New MonsterCollection", menuName = "Custom/MonsterCollection")]
public class MonsterCollection : ScriptableObject
{
    [SerializeField] Monster[] monsters;

    public Monster[] Monsters => monsters;

    public Monster GetMonsterById(int id)
    {
        return monsters.FirstOrDefault(monsters => monsters.id == id);
    }

#if UNITY_EDITOR
    public void AddMonster(Monster newMonster)
    {
        monsters = new List<Monster>(monsters){newMonster}.ToArray();
        EditorUtility.SetDirty(this);
    }

    public void RemoveMonster(Monster removedMonster)
    {
        var monsterList = new List<Monster>(monsters);
        monsterList.Remove(removedMonster);
        monsters = monsterList.ToArray();
        EditorUtility.SetDirty(this);
    }
#endif
}
