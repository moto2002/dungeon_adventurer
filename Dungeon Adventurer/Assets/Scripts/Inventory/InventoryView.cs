using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : View
{
    const int RowsPerColumn = 8;

    [SerializeField] Transform container;
    [SerializeField] ItemController prefab_Item;
    [SerializeField] Slot prefab_Slot;
    [SerializeField] ItemInfoView itemInfo;

    private List<Item> _items;
    FilterMode _filter = FilterMode.Cost;
    bool _orderDesc = true;

    Hero _currentHero;

    void Init()
    {
        var length = _items.Count;

        var columnCount = length / RowsPerColumn;

        if (length % RowsPerColumn != 0) columnCount++;

        Slot[] slots = new Slot[columnCount * RowsPerColumn];

        for (int i = 0; i < columnCount * RowsPerColumn; i++)
        {
            var slot = Instantiate(prefab_Slot, container);
            slots[i] = slot;
        }

        List<GameObject> itemControllers = new List<GameObject>();
        var orderedList = _items;
        switch (_filter)
        {
            case FilterMode.None:
                break;
            case FilterMode.Cost:
                orderedList = _orderDesc ? _items.OrderByDescending(e => e.price).ToList() : _items.OrderBy(e => e.price).ToList();
                break;
            case FilterMode.Rarity:
                orderedList = _orderDesc ? _items.OrderByDescending(e => e.rarity).ToList() : _items.OrderBy(e => e.rarity).ToList();
                break;
            case FilterMode.Type:
                orderedList = _orderDesc ? _items.OrderByDescending(e => e.type).ToList() : _items.OrderBy(e => e.type).ToList();
                break;
        }

        foreach (var item in orderedList)
        {
            var itemController = Instantiate(prefab_Item);
            itemController.SetData(item, () => itemInfo.SetData(_currentHero, item, false));
            itemControllers.Add(itemController.gameObject);
        }

        for (int i = 0; i < itemControllers.Count; i++)
        {
            slots[i].item = itemControllers[i];
        }
    }

    public void Refresh(Hero hero)
    {
        _currentHero = hero;
    }

    public void SetFilter(int filter)
    {
        var newFilter = (FilterMode)filter;
        if (_filter == newFilter)
        {
            _orderDesc = !_orderDesc;
        }
        else
        {
            _orderDesc = true;
            _filter = newFilter;
        }
        RemoveInventoryItems();
        Init();
    }

    void RemoveInventoryItems()
    {
        foreach (Transform trans in container)
        {
            Destroy(trans.gameObject);
        }
    }

    public void RefreshInventory(InventoryModel inventory)
    {
        var list = inventory.InventoryItems.items;
        if (list == null) return;

        var itemList = new List<Item>();

        foreach (var itemData in list)
        {
            var item = ScriptableObject.CreateInstance<Item>();
            item.SetData(itemData);
            itemList.Add(item);
        }
        _items = itemList;
        RemoveInventoryItems();
        Init();
    }
}

public enum FilterMode
{
    None,
    Cost,
    Rarity,
    Type,
}
