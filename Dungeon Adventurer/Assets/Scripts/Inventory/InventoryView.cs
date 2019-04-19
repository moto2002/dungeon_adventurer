using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    private Item[] _items;

    [SerializeField] Transform container;
    [SerializeField] ItemController prefab_Item;
    [SerializeField] Slot prefab_Slot;

    [SerializeField] ItemInfoView itemInfo;


    public void SetData(Item[] items) {

        _items = items;
        Init();

    }

    void Init() {

        var length = _items.Length;

        var columnCount = length / 3;

        if (length % 3 != 0) columnCount++;

        Slot[] slots = new Slot[columnCount * 3];

        for (int i = 0; i < columnCount*3; i++)
        {
             var slot =Instantiate(prefab_Slot, container);
            slots[i] = slot;
        }

        List<GameObject> itemControllers = new List<GameObject>();


        foreach (var item in _items) {

            var i = Instantiate(prefab_Item);
            i.SetData(item, delegate { itemInfo.SetData(item);});
            itemControllers.Add(i.gameObject);
        }

        for (int i = 0; i < itemControllers.Count; i++)
        {
            slots[i].item = itemControllers[i];
        }
    }
}
