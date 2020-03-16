using System;
using System.Collections.Generic;

public class CurrencyModel
{
    Dictionary<Currency, double> CurrencyAmount;

    public CurrencyModel(CurrencyData data)
    {
        CurrencyAmount = new Dictionary<Currency, double>();

        AddCurrency(Currency.Coins, data.coinAmount);
        AddCurrency(Currency.Diamonds, data.diamondsAmount);
    }

    public CurrencyData GetCurrencyAsData()
    {
        return new CurrencyData()
        {
            coinAmount = CurrencyAmount[Currency.Coins],
            diamondsAmount = CurrencyAmount[Currency.Diamonds]
        };
    }

    void AddCurrency(Currency cur, double value)
    {
        CurrencyAmount.Add(cur, value);
    }

    public double GetCurrency(Currency cur)
    {
        return CurrencyAmount[cur];
    }

    public bool TryPurchase(Currency cur, double value)
    {
        if (CheckCurrency(cur, value))
        {
            DecreaseCurrency(cur, value);
            return true;
        } else
        {
            return false;
        }
    }

    public void CreditCurrency(Currency cur, double value)
    {
        IncreaseCurrency(cur, value);
    }

    void IncreaseCurrency(Currency cur, double value)
    {
        CurrencyAmount[cur] += value;
        DataHolder._data.SaveCurrency(GetCurrencyAsData());
    }

    void DecreaseCurrency(Currency cur, double value)
    {
        CurrencyAmount[cur] -= value;
        DataHolder._data.SaveCurrency(GetCurrencyAsData());
    }

    bool CheckCurrency(Currency c, double amount)
    {
        return CurrencyAmount[c] >= amount;
    }

    public CoinData ConvertCoin()
    {

        var amount = CurrencyAmount[Currency.Coins];
        return ConvertDoubleToCoinData(amount);
    }

    public CoinData ConvertDoubleToCoinData(double amount)
    {
        var gold = amount / 10000;
        amount -= (int)gold * 10000;
        var silver = amount / 100;
        amount -= (int)silver * 100;
        var copper = amount / 1;
        amount -= (int)copper * 1;

        return new CoinData()
        {
            gold = (int)gold,
            silver = (int)silver,
            copper = (int)copper
        };
    }
}

public enum Currency
{
    None,
    Coins, //Gold/Silver/Copper
    Diamonds,
}

public struct CoinData
{
    public int gold;
    public int silver;
    public int copper;
}

[Serializable]
public struct CurrencyData
{
    public double coinAmount;
    public double diamondsAmount;
}
