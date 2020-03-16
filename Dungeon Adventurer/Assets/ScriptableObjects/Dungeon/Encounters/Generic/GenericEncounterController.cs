using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static EncounterResultController;

public class GenericEncounterController : UIBehaviour
{
    [SerializeField] TextMeshProUGUI description;

    [SerializeField] OptionButton prefabOptionButton;
    [SerializeField] Transform buttonsContainer;
    [SerializeField] EncounterResultController resultController;

    GenericEncounter _encounterDef;
    Hero[] _heroes;

    public void SetData(GenericEncounter def, Hero[] heroes, Action<Option> optionCallback)
    {
        _encounterDef = def;
        _heroes = heroes;

        description.text = _encounterDef.Description;
        _encounterDef.Options.ForEach(option => 
        {
            var button = Instantiate(prefabOptionButton, buttonsContainer);
            button.SetData(option, optionCallback, _heroes);
        });
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
