using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New BaseItemManager", menuName = "Custom/BaseItemManager")]
public class BaseItemManager : ScriptableObject
{
    [SerializeField] BaseItemCollection[] collections;
    public BaseItemCollection[] Collections => collections;

    public BaseItemCollection GetCollectionByMainStat(MainStat stat)
    {
        return collections.FirstOrDefault(e => e.MainStat == stat);
    }

    public BaseItem GetRandomItem() {

        return collections[Random.Range(0, collections.Length)].GetRandomItem();
    }

    public BaseItem GetItemById(int id)
    {
        foreach (var collection in collections) {

            var item = collection.GetItemById(id);
            if (item == null) continue;
            return item;
        }
        Debug.LogWarning($"RRRR Return without Value id: {id}");
        return null;
    }
}
