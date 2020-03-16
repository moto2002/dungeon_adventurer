using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static EncounterResultController;

public class ObstacleEncounterController : UIBehaviour
{
    [SerializeField] TextMeshProUGUI description;

    [SerializeField] OptionButton noReqButton;
    [SerializeField] OptionButton firstSkillButton;
    [SerializeField] OptionButton secondSkillButton;
    [SerializeField] Button cancelButton;

    [SerializeField] EncounterResultController resultController;

    ObstacleEncounter _encounterDef;
    Hero[] _heroes;

    protected override void Awake()
    {
        cancelButton.onClick.AddListener(CloseEncounter);
    }

    public void SetData(ObstacleEncounter def, Hero[] heroes, Action<Option> optionCallback)
    {
        _encounterDef = def;
        _heroes = heroes;

        description.text = _encounterDef.Description;

        noReqButton.SetData(def.NoReqOption, optionCallback, heroes);
        firstSkillButton.SetData(def.FirstSkillOption, optionCallback, heroes);
        secondSkillButton.SetData(def.SecondSkillOption, optionCallback, heroes);
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
