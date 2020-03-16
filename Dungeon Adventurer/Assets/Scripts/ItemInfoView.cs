using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoView : UIBehaviour, IPointerClickHandler
{
    const int ButtonHeight = 80;

    [SerializeField] ItemInfoStats selectedItemStats;
    [SerializeField] bool targetInventory;

    [Header("Buttons")]
    [SerializeField] Button sellButton;
    [SerializeField] Button equipButton;
    [SerializeField] Button closeButton;

    [Header("Sizing")]
    [SerializeField] RectTransform itemTransform;
    [SerializeField] RectTransform displayTransform;

    [SerializeField] TextMeshProUGUI equipText;

    Item _shownItem;
    Hero _currentHero;
    int _resize;

    protected override void Awake()
    {
        sellButton.onClick.AddListener(Sell);
        closeButton.onClick.AddListener(CloseItemInfo);
    }

    void LateUpdate()
    {
        if (_resize < 2)
        {
            _resize++;
            if (_resize >= 2)
            {
                var height = itemTransform.rect.height + ButtonHeight;
                displayTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }

    public void SetData(Hero currentHero, Item item, bool equipped)
    {
        _shownItem = item;
        _currentHero = currentHero;

        if (equipped)
        {
            SetEquipButtonCallback(UnEquip);
            equipText.text = "Drop";
            selectedItemStats.SetData(item);
        } else
        {
            SetEquipButtonCallback(Equip);
            equipText.text = "Equip";
            selectedItemStats.SetData(item, currentHero);
        }
        _resize = 0;
        gameObject.SetActive(true);
    }

    void SetEquipButtonCallback(UnityAction callback)
    {
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(callback);
    }

    void Sell()
    {
        if (targetInventory)
        {
            ServiceRegistry.Inventory.SellItem(_shownItem.GetItemData(), _shownItem.price);
        } else
        {
            ServiceRegistry.Dungeon.SellItem(_shownItem.GetItemData(), _shownItem.price);
        }
        CloseItemInfo();
    }

    void Equip()
    {
        if (targetInventory)
        {
            ServiceRegistry.Characters.EquipItemInventory(_shownItem, _currentHero);
        } else
        {
            ServiceRegistry.Characters.EquipItemBackpack(_shownItem, _currentHero);
        }
        CloseItemInfo();
    }

    void UnEquip()
    {
        if (targetInventory)
        {
            ServiceRegistry.Characters.UnEquipItemInventory(_shownItem, _currentHero);
        } else
        {
            ServiceRegistry.Characters.UnEquipItemBackpack(_shownItem, _currentHero);
        }
        CloseItemInfo();
    }

    void CloseItemInfo()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CloseItemInfo();
    }
}
