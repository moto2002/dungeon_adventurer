using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterBackpack : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI charName;
    [SerializeField] Image charIcon;
    [SerializeField] Transform container;
    [SerializeField] ItemController prefab_Item;
    [SerializeField] Slot prefab_Slot;

    ItemInfoView _itemInfo;
    int _slotCount;
    private List<Item> _items;
    FilterMode _filter = FilterMode.Cost;
    bool _orderDesc = true;

    Hero _currentHero;
    Hero _assignedHero;

    void RefreshSlots()
    {
        foreach (Transform trans in container)
        {
            if (trans.name.Equals("Header")) continue;
            Destroy(trans.gameObject);
        }

        Slot[] slots = new Slot[_slotCount];

        for (int i = 0; i < _slotCount; i++)
        {
            var slot = Instantiate(prefab_Slot, container);
            slots[i] = slot;
        }

        if (_items == null) return;

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
            var i = Instantiate(prefab_Item);
            i.SetData(item, () => { _itemInfo.SetData(_currentHero, item, false); });
            itemControllers.Add(i.gameObject);
        }

        for (int i = 0; i < itemControllers.Count; i++)
        {
            slots[i].item = itemControllers[i];
        }
    }

    public void Refresh(Hero hero, ItemInfoView itemInfo)
    {
        _currentHero = hero;
        _itemInfo = itemInfo;
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
        RefreshSlots();
    }

    public void RefreshItems(Hero hero, int slots, ItemData[] items)
    {
        _slotCount = slots;
        _assignedHero = hero;

        charName.text = hero.displayName;
        charIcon.sprite = DataHolder._data.raceImages[(int)hero.race];

        var itemList = new List<Item>();
        foreach (var itemData in items)
        {
            var item = ScriptableObject.CreateInstance<Item>();
            item.SetData(itemData);
            itemList.Add(item);
        }
        _items = itemList;
        RefreshSlots();
    }
}
