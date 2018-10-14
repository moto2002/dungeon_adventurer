using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterCreateData
{
    public CharacterCreate[] characterCreates;
}

[System.Serializable]
public class CharacterCreate
{
    public Rarity rarity;
    public int startingPoints;
    public float[] growth;
}
