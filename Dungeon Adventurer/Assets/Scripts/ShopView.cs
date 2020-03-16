using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopView : View, CurrencyService.OnCurrencyChanged, TimeProvider.OnTimeChanged
{
    const int ENTRY_COUNT = 6;
    public string TimeKey => "RefreshTimer";
    int waitTime = 30;

    [SerializeField] ShopEntryView prefabShopEntry;
    [SerializeField] Transform shopContainer;
    [SerializeField] ItemInfoStats statsDisplay;
    [SerializeField] Button rerollButton;
    [SerializeField] TextMeshProUGUI rerollLabel; 
    [SerializeField] Button closeButton;

    CurrencyModel _currencyModel;


    protected override void Awake()
    {
        rerollButton.onClick.AddListener(Reroll);
        closeButton.onClick.AddListener(() => ViewUtility.Hide<ShopView>());
    }

    void Reroll()
    {
        TimeProvider.RegisterTime(TimeKey, new TimeSpan(0, 0, waitTime));
        ClearContainer();
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < ENTRY_COUNT; i++)
        {
            var itemData = ItemCreator.CreateItem(UnityEngine.Random.Range(1, 20));
            var item = ScriptableObject.CreateInstance<Item>();
            item.SetData(itemData);
            var entry = Instantiate(prefabShopEntry, shopContainer);
            entry.transform.localScale = Vector3.one;
            entry.SetData(itemData, () => { ShowDisplay(item); }, _currencyModel);
            if (i == 0) ShowDisplay(item);
        }
    }

    void ShowDisplay(Item display)
    {
        statsDisplay.SetData(display);
    }

    void ClearContainer()
    {
        foreach (Transform trans in shopContainer)
        {
            Destroy(trans.gameObject);
        }
    }

    public void OnModelChanged(CurrencyModel model)
    {
        _currencyModel = model;
        ClearContainer();
        Refresh();
    }

    public void OnTimeChanged(TimeProvider.TimeModel model)
    {
        if(model.RemainingTime.TotalSeconds <= 0)
        {
            rerollButton.interactable = true;
            rerollLabel.text = "Reroll";
        } else
        {
            rerollButton.interactable = false;
            rerollLabel.text = $"Wait " + model.RemainingTime.Stringify();
        }
    }
}
