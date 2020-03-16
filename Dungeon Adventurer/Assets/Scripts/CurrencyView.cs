using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyView : EventListener, CurrencyService.OnCurrencyChanged
{
    [SerializeField] TextMeshProUGUI goldAmount;
    [SerializeField] TextMeshProUGUI silverAmount;
    [SerializeField] TextMeshProUGUI copperAmount;
    [SerializeField] TextMeshProUGUI diamondsAmount;

    CurrencyModel _model;

    void UpdateCurrency(Currency cur, double value, int change) {

        switch (cur) {
            case Currency.Coins:
                var data = _model.ConvertCoin();
                if (goldAmount)
                    goldAmount.text = $"{data.gold}g";
                if (silverAmount)
                    silverAmount.text = $"{data.silver:D2}s";
                if (copperAmount)
                    copperAmount.text = $"{data.copper:D2}c";
                break;
            case Currency.Diamonds:
                if (!diamondsAmount) return;
                diamondsAmount.text = $"{value}";
                break;
        }

        if (change == 0) return;
    }

    public void OnModelChanged(CurrencyModel model)
    {
        _model = model;
        var curData = _model.GetCurrencyAsData();

        UpdateCurrency(Currency.Coins, curData.coinAmount, 0);
        UpdateCurrency(Currency.Diamonds, curData.diamondsAmount, 0);
    }
}
