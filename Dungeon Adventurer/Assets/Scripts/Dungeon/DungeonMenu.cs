using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField minLevel;
    [SerializeField] TMP_InputField maxLevel;
    [SerializeField] TMP_Dropdown dungeonPicker;
    [SerializeField] Button enterDungeon;

    Dictionary<int, Dungeon> dungeonDict;
    int _selectedValue = 1;

    private void OnEnable()
    {
        dungeonPicker.onValueChanged.AddListener(OnChangeSelection);
        enterDungeon.onClick.AddListener(EnterDungeon);
    }

    public void Init()
    {
        dungeonDict = DataHolder._data.DungeonDict;

        dungeonPicker.ClearOptions();

        List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();
        foreach (var dungeon in dungeonDict)
        {
            data.Add(new TMP_Dropdown.OptionData(dungeon.Value.displayName));
        }
        dungeonPicker.AddOptions(data);
    }

    private void OnDisable()
    {
        dungeonPicker.onValueChanged.RemoveListener(OnChangeSelection);
        enterDungeon.onClick.RemoveListener(EnterDungeon);
    }

    public void OnChangeSelection(int value)
    {
        _selectedValue = value + 1;
        minLevel.text = $"{dungeonDict[value + 1].minLvl}";
        maxLevel.text = $"{dungeonDict[value + 1].maxLvl}";
        ServiceRegistry.Dungeon.SetSelectedId(_selectedValue);
    }

    public void EnterDungeon()
    {
        ServiceRegistry.Dungeon.StartDungeon();
        ViewUtility.ShowThenHide<DungeonView, BattleOrderView>();
    }
}
