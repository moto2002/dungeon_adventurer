#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "New BaseItem", menuName = "Custom/BaseItem")]
public class BaseItem : ScriptableObject
{
    const int ArmorIconPerLevel = 15;
    const int WeaponIconPerLevel = 10;

    [SerializeField] int id;
    [SerializeField] ItemType type;
    [SerializeField] Sprite[] icons;
    [SerializeField] ItemStatFocus[] statFocus;

    public int Id => id;
    public ItemType Type => type;
    public Sprite[] Icons => icons;
    public ItemStatFocus[] StatFocus => statFocus;

    public Sprite GetIcon(int level)
    {
        var iconUsed = level / (type == ItemType.Weapon ? WeaponIconPerLevel : ArmorIconPerLevel);
        if (iconUsed >= icons.Length && icons.Length != 0)
        {
            iconUsed = icons.Length - 1;
        }
        return icons != null && icons.Length > iconUsed ? icons[iconUsed] : null;
    }

#if UNITY_EDITOR
    public void SetIcon(int id, Sprite sprite)
    {
        if (icons == null)
        {
            icons = new Sprite[1];
        }

        if (icons.Length <= id)
        {
            var oldList = icons;
            icons = new Sprite[id + 1];
            for (var i = 0; i < oldList.Length; i++)
            {
                icons[i] = oldList[i];
            }
        }

        icons[id] = sprite;
        EditorUtility.SetDirty(this);
    }
#endif
}
