using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleRewardController : UIBehaviour
{
    [Header("Feedback")]
    [SerializeField] GameObject finishFeedback;
    [SerializeField] Button winbutton;
    [SerializeField] Slot[] rewardSlots;
    [SerializeField] ItemController itemPrefab;
    [SerializeField] ItemInfoStats itemInfo;

    [SerializeField] Button takeButton;
    [SerializeField] Button takeAllButton;
    [SerializeField] Button cleanInventoryButton;

    int _openSlots = 0;
    Item _currentSelectedItem;
    List<Item> _items = new List<Item>();

    protected override void Awake()
    {
        winbutton.onClick.AddListener(() => Close());
        takeButton.onClick.AddListener(() => TakeItem());
        takeAllButton.onClick.AddListener(() => TakeAllItems());
    }

    public void SetBattleResult(BattleResult result)
    {
        rewardSlots.ToList().ForEach(slot => {
            if (slot.item != null)
            {
                Destroy(slot.item);
            }
        });

        ServiceRegistry.Currency.CreditCurrency(Currency.Coins, result.Gold);
        ServiceRegistry.Characters.ApplyExperience(result.Experience);

        if (result.Victorious)
        {
            finishFeedback.SetActive(true);

            for (var i = 0; i < result.Items.Count; i++)
            {
                var item = ScriptableObject.CreateInstance<Item>();
                item.SetData(result.Items[i]);
                _items.Add(item);

                var itemController = Instantiate(itemPrefab);
                itemController.SetData(item, () => {

                    itemInfo.SetData(item);
                    itemInfo.gameObject.SetActive(true);
                    _currentSelectedItem = item;
                });
                rewardSlots[i].item = itemController.gameObject;
            }
        }
    }

    void Close()
    {
        finishFeedback.SetActive(false);
        itemInfo.gameObject.SetActive(false);
    }

    void TakeItem()
    {
        if (ServiceRegistry.Dungeon.AddItemToBackpack(_currentSelectedItem.GetItemData()))
        {
            rewardSlots.ToList().ForEach(slot => {
                if (slot.item != null && slot.item.GetComponent<ItemController>().AppliedItem == _currentSelectedItem)
                {
                    Destroy(slot.item);
                }
            });
            itemInfo.gameObject.SetActive(false);
            _items.Remove(_currentSelectedItem);
            if (_items.Count == 0) Close();

        }
    }

    void TakeAllItems()
    {
        var items = new List<Item>(_items);
        for (var i = 0; i < Mathf.Min(items.Count, _openSlots); i++)
        {
            _currentSelectedItem = items[i];
            TakeItem();
        }
        if (_items.Count == 0) Close();
    }

    public void RefreshOpenSlots(int value)
    {
        _openSlots = value;
        if(_openSlots <= 0)
        {
            takeButton.interactable = false;
            takeAllButton.interactable = false;
        }
    }
}
