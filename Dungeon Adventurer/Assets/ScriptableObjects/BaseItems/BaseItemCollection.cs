using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New BaseItemCollection", menuName = "Custom/BaseItemCollection")]
public class BaseItemCollection : ScriptableObject
{
    [SerializeField] MainStat mainStat;
    [SerializeField] BaseItem[] items;
    [SerializeField] ItemStatFocus statFocus;

    public BaseItem[] Items => items;
    public MainStat MainStat => mainStat;

    public BaseItem GetRandomItem()
    {
        return items[Random.Range(0, items.Length)];
    }

    public BaseItem GetItemById(int id) {
        return items.FirstOrDefault(e => e.Id == id);
    }
}
