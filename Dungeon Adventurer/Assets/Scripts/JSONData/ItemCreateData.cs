using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemCreateData
{
    public ItemCreate[] itemCreates;
}

[System.Serializable]
public class ItemCreate
{
    public Rarity rarity;
    public int mainRange;
    public int mainAmount;
    public int subRange;
    public int subAmount;
}
