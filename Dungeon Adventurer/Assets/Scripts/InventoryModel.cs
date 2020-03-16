using System.Collections.Generic;

public class InventoryModel
{
    public ItemContainer InventoryItems => _inventoryItems;

    ItemContainer _inventoryItems;

    public InventoryModel(ItemContainer container)
    {
        _inventoryItems = container;
    }

    public void AddItem(ItemData data) {
        var list = _inventoryItems.items != null ? new List<ItemData>(_inventoryItems.items) : new List<ItemData>();
        list.Add(data);
        _inventoryItems.items = list.ToArray();
    }

    public void RemoveItem(ItemData data)
    {
        var list = new List<ItemData>(_inventoryItems.items);
        if (!list.Contains(data)) return;

        list.Remove(data);
        _inventoryItems.items = list.ToArray();
    }
}
