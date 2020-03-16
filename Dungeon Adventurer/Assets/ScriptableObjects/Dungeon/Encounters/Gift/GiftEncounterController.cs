using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static EncounterResultController;

public class GiftEncounterController : UIBehaviour
{
    [SerializeField] TextMeshProUGUI description;

    [SerializeField] OptionButton takeButton;
    [SerializeField] Button cancelButton;

    [SerializeField] EncounterResultController resultController;

    AGiftEncounter _encounterDef;
    Hero[] _heroes;

    protected override void Awake()
    {
        cancelButton.onClick.AddListener(CloseEncounter);
    }

    public void SetData(AGiftEncounter def, Action<Option> optionCallback, params Hero[] heroes)
    {
        _encounterDef = def;
        _heroes = heroes;

        description.text = _encounterDef.Description;

        takeButton.SetData(def.TakeOption, optionCallback, heroes);
    }

    public void ShowResult(EncounterResult result)
    {
        resultController.ShowResult(result, CloseEncounter);
    }

    public void CloseEncounter()
    {
        gameObject.SetActive(false);
    }
}
