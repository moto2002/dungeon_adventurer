using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI type;
    [SerializeField] TextMeshProUGUI value;

    [SerializeField] Image rarityBorder;

    [SerializeField] ItemInfoStat prefab_Stats;
    [SerializeField] Transform mainContainer;
    [SerializeField] Transform subContainer;

    private Item _shownItem;

    public void SetData(Item item)
    {
        _shownItem = item;
        Init();
    }

    void Init()
    {
        name.text = _shownItem.itemName;
        var color = Colors.ByRarity(_shownItem.rarity);
        name.color = color;
        rarityBorder.color = color;

        type.text = _shownItem.type.ToString();
        value.text = "100$";

        ApplyMainStats(_shownItem.main.ToArray());
        ApplySubStats(_shownItem.sub.ToArray());
    }

    void ApplyMainStats(ItemMainStatValue[] stats)
    {
        foreach (Transform g in mainContainer.transform) {
            Destroy(g.gameObject);
        }

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

        foreach (var stat in stats)
        {
            var s = Instantiate(prefab_Stats, subContainer);
            s.SetData(stat);
        }
    }
}
