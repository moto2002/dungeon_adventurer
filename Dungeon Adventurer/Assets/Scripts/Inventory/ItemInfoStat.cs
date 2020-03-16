using TMPro;
using UnityEngine;

public class ItemInfoStat : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI type;
    [SerializeField] TextMeshProUGUI value;
    public void SetData(ItemMainStatValue stats)
    {
        type.text = stats.StatToShortString();
        value.text = $"{stats.value}";

        var color = Colors.ByMainStat(stats.stat);
        type.color = color;
        value.color = color;
    }

    public void SetData(ItemSubStatValue stats)
    {
        type.text = stats.stat.ToString();
        value.text = SubStats.GetSimpleString(stats.stat, stats.value);
    }
}
