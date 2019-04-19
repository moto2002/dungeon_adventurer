using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int id;
    public ItemType slot;
    public ItemMainStatValue[] mainStatChanges;
    public ItemSubStatValue[] subStatChanges;
}

