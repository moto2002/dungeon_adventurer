using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI type;
    [SerializeField] TextMeshProUGUI pLevel;
    [SerializeField] TextMeshProUGUI level;

    [SerializeField] TextMeshProUGUI goldAmount;
    [SerializeField] TextMeshProUGUI silverAmount;
    [SerializeField] TextMeshProUGUI copperAmount;

    [SerializeField] Image rarityBorder;

    [SerializeField] ItemInfoStat prefab_Stats;
    [SerializeField] Transform mainContainer;
    [SerializeField] Transform subContainer;

    [SerializeField] GameObject changes;
    [SerializeField] TextMeshProUGUI dmgChange;
    [SerializeField] TextMeshProUGUI toughChange;
    [SerializeField] TextMeshProUGUI utilityChange;

    Item _shownItem;
    Hero _selectedHero;

    public void SetData(Item item)
    {
        _shownItem = item;
        _selectedHero = null;

        Init();
    }

    public void SetData(Item item, Hero selectedHero)
    {
        _shownItem = item;
        _selectedHero = selectedHero;

        Init();
    }

    void Init()
    {
        name.text = _shownItem.itemName;
        var color = Colors.ByRarity(_shownItem.rarity);
        name.color = color;
        rarityBorder.color = color;

        type.text = _shownItem.type.ToString();
        var coinData = ServiceRegistry.Currency.ConvertDoubleToCoinData(_shownItem.price);

        ApplyCurrency(goldAmount, coinData.gold, "g");
        ApplyCurrency(silverAmount, coinData.silver, "s");
        ApplyCurrency(copperAmount, coinData.copper, "c");

        pLevel.text = _shownItem.powerLevel.ToString();
        level.text = _shownItem.level.ToString();

        ApplyMainStats(_shownItem.main.ToArray());
        ApplySubStats(_shownItem.sub.ToArray());

        if (_selectedHero != null)
        {
            var oldAverages = new Tuple<int, int, int>(_selectedHero.AverageDamage, _selectedHero.AverageToughness, _selectedHero.AverageUtility);
            var newAverages = _selectedHero.GetAverages(_shownItem);
            var differenceTuple = new Tuple<float, float, float>(newAverages.Item1 / (float)oldAverages.Item1 - 1f, newAverages.Item2 / (float)oldAverages.Item2 - 1f, newAverages.Item3 / (float)oldAverages.Item3 - 1f);

            dmgChange.text = differenceTuple.Item1 == 0 ? $"<color={Colors.ByValue(differenceTuple.Item1)}>-</color>" : $"<color={Colors.ByValue(differenceTuple.Item1)}>{differenceTuple.Item1 * 100f:N0} %</color>";
            toughChange.text = differenceTuple.Item2 == 0 ? $"<color={Colors.ByValue(differenceTuple.Item2)}>-</color>" : $"<color={Colors.ByValue(differenceTuple.Item2)}>{differenceTuple.Item2 * 100f:N0} %</color>";
            utilityChange.text = differenceTuple.Item3 == 0 ? $"<color={Colors.ByValue(differenceTuple.Item3)}>-</color>" : $"<color={Colors.ByValue(differenceTuple.Item3)}>{differenceTuple.Item3 * 100f:N0} %</color>";
        }

        changes.SetActive(_selectedHero != null);
    }

    void ApplyCurrency(TextMeshProUGUI text, int amount, string currencyTag)
    {
        if (text)
        {
            text.text = $"{amount}" + currencyTag;
            if (amount <= 0) text.enabled = false;
            else
                text.enabled = true;
        }
    }

    void ApplyMainStats(ItemMainStatValue[] stats)
    {
        foreach (Transform g in mainContainer.transform)
        {
            Destroy(g.gameObject);
        }

        if (stats.Length > 0)
            foreach (var stat in stats)
            {
                var s = Instantiate(prefab_Stats, mainContainer);
                s.SetData(stat);
            }

    }
    void ApplySubStats(ItemSubStatValue[] stats)
    {
        foreach (Transform g in subContainer.transform)
        {
            Destroy(g.gameObject);
        }

        if (stats.Length > 0)
            foreach (var stat in stats)
            {
                var s = Instantiate(prefab_Stats, subContainer);
                s.SetData(stat);
            }
    }
}
