using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static EncounterResultController;

public class TrapEncounterController : UIBehaviour
{
    [SerializeField] TextMeshProUGUI description;

    [SerializeField] OptionButton firstSkillButton;
    [SerializeField] OptionButton secondSkillButton;
    [SerializeField] EncounterResultController resultController;
    [SerializeField] TextMeshProUGUI heroName;
    [SerializeField] SimpleCharacterPlate heroPlate;

    TrapEncounter _encounterDef;
    Hero _hero;

    public void SetData(TrapEncounter def, Hero hero, Action<Option> optionCallback)
    {
        _encounterDef = def;
        _hero = hero;

        heroPlate.SetData(hero, ()=> { });
        heroName.text = hero.displayName;

        description.text = _encounterDef.Description;

        firstSkillButton.SetData(def.FirstSkillOption, optionCallback, _hero);
        secondSkillButton.SetData(def.SecondSkillOption, optionCallback, _hero);
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
