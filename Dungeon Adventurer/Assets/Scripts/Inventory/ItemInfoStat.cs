using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInfoStat : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI type;
    [SerializeField] TextMeshProUGUI value;

    public void SetData(ItemMainStatValue stats) {

        type.text = stats.StatToShortString();
        value.text = $"+ {stats.value}";

        Debug.Log("MainStat");

        var color = Colors.ByMainStat(stats.stat);
        type.color = color;
        Debug.Log(type.color.g);
        value.color = color;
    }

    public void SetData(ItemSubStatValue stats)
    {
        type.text = stats.stat.ToString();
        value.text = $"+ {stats.value}";
    }


}
