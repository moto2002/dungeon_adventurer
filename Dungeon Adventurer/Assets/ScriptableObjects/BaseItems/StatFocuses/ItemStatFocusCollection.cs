using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New ItemStatFocusCollection", menuName = "Custom/StatFocusCollection")]
public class ItemStatFocusCollection : ScriptableObject
{
    [SerializeField] ItemStatFocus[] focuses;

    public ItemStatFocus[] Focuses => focuses;

    public ItemStatFocus GetFocusByStats(MainStat primary, MainStat second)
    {
        return focuses.FirstOrDefault(focus => focus.focus[0] == primary && focus.focus[1] == second);
    }
}
