using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackpackModel
{
    public Dictionary<int, ItemData[]> InventoryItems => _inventoryItems;
    public Dictionary<int, int> PossibleSlots => _possibleSlots;

    readonly Dictionary<int, ItemData[]> _inventoryItems = new Dictionary<int, ItemData[]>();
    readonly Dictionary<int, int> _possibleSlots = new Dictionary<int, int>();

    public int OpenSlots()
    {
        var amount = 0;

        foreach(var entry in InventoryItems)
        {
            var dif = PossibleSlots[entry.Key] - entry.Value.Length;
            if (dif > 0) amount += dif;
        }

        return amount;
    }

    public BackpackModel(Dictionary<int, int> container)
    {
        _inventoryItems.Clear();
        _possibleSlots = container;
        foreach (var c in _possibleSlots)
        {
            _inventoryItems.Add(c.Key, new ItemData[] { });
        }
    }

    public bool HasEmptySlot(int id)
    {
        if (_inventoryItems.TryGetValue(id, out var slots))
        {
            if (slots.Length >= _possibleSlots[id])
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public bool AddItem(ItemData data)
    {
        foreach(var charID in _inventoryItems.Keys)
        {
            if (AddItem(charID, data)) return true;
        }

        return false;
    }

    public bool RemoveItem(ItemData data)
    {
        foreach (var charID in _inventoryItems.Keys)
        {
            if (RemoveItem(charID, data)) return true;
        }

        return false;
    }

    public bool AddItem(int charId, ItemData data)
    {
        if (!HasEmptySlot(charId)) return false;

        var list = _inventoryItems[charId].ToList();
        list.Add(data);
        _inventoryItems[charId] = list.ToArray();
        return true;
    }

    public bool RemoveItem(int charId, ItemData data)
    {
        var list = new List<ItemData>(_inventoryItems[charId]);
        if (!list.Contains(data)) return false;

        list.Remove(data);
        _inventoryItems[charId] = list.ToArray();
        return true;
    }
}
