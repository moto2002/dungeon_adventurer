using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New DungeonCollection", menuName = "Custom/DungeonCollection")]
public class DungeonCollection : ScriptableObject
{
    [SerializeField] Dungeon[] dungeons;

    public Dungeon[] Dungeons => dungeons;

    public Dungeon GetMonsterById(int id)
    {
        return dungeons.FirstOrDefault(dungeons => dungeons.id == id);
    }

#if UNITY_EDITOR
    public void AddDungeon(Dungeon newDungeon)
    {
        dungeons = new List<Dungeon>(dungeons) { newDungeon }.ToArray();
        EditorUtility.SetDirty(this);
    }

    public void RemoveDungeon(Dungeon removedDungeon)
    {
        var dungeonList = new List<Dungeon>(dungeons);
        dungeonList.Remove(removedDungeon);
        dungeons = dungeonList.ToArray();
        EditorUtility.SetDirty(this);
    }
#endif
}
