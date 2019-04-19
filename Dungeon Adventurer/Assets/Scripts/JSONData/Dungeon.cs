using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Custom/Dungeon")]
public class Dungeon : ScriptableObject
{
    public int id;
    public string name;
    public int minLvl;
    public int maxLvl;

    public Monster[] possibleMonsters;
    public Monster boss;
}

