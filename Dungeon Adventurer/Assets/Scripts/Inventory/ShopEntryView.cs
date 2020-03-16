using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopEntryView : MonoBehaviour
{
    [SerializeField] ItemController prefabItem;
    [SerializeField] Transform container;
    [SerializeField] Button buyButton;

    Item _item;

    public void SetData(ItemData it, Action callback, CurrencyModel model) {

        _item = ScriptableObject.CreateInstance<Item>();
        _item.SetData(it);
        var itemEntry = Instantiate(prefabItem, container);
        itemEntry.transform.localScale = new Vector3(1, 1, 1);
        itemEntry.SetData(_item, callback);
        buyButton.onClick.AddListener(() => { BuyItem(it); });
        buyButton.interactable = _item.price <= model.GetCurrency(Currency.Coins);
    }

    void Refresh(Currency cur, double value, int change) {

        if (cur != Currency.Coins) return;
        buyButton.interactable = _item.price <= value;
    }

    void BuyItem(ItemData data) {
        if (!ServiceRegistry.Currency.TryPurchase(Currency.Coins, _item.price)) return;
        ServiceRegistry.Inventory.AddItemToInventory(data);
        Destroy(gameObject);
    }
}
