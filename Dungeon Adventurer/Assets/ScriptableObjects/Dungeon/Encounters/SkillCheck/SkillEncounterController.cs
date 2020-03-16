using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SkillEncounterController : UIBehaviour
{
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI requiredValue;

    [SerializeField] Button startCheck;
    [SerializeField] Button cancelCheck;

    [SerializeField] Transform[] slots;
    [SerializeField] SimpleCharacterPlate platePrefab;

    SkillEncounter _encounterDef;
    Hero[] _heroes;
    Hero _choosenHero;
    Action _successCall;
    Action _failCall;

    protected override void Awake()
    {
        cancelCheck.onClick.AddListener(CancelEncounter);
    }

    public void SetData(SkillEncounter def, Hero[] heroes, int encounterLevel, Action checkCallback)
    {
        _encounterDef = def;
        _heroes = heroes;

        description.text = _encounterDef.Description();
        cancelCheck.gameObject.SetActive(_encounterDef.Optional);
        startCheck.onClick.AddListener(checkCallback.Invoke);

        ChooseHero();
    }

    void ChooseHero()
    {
        switch (_encounterDef.CheckType)
        {
            case CheckType.Group:
                int i;
                for(i = 0; i< _heroes.Length; i++)
                {
                    var hero = _heroes[i];
                    var plate = Instantiate(platePrefab, slots[i]);
                    plate.SetData(hero, () => { });
                    slots[i].gameObject.SetActive(true);
                }
                for(var u = i; u < slots.Length; u++)
                {
                    slots[u].gameObject.SetActive(false);
                }

                break;
            case CheckType.Single:
                var hero1 = _heroes[UnityEngine.Random.Range(0, _heroes.Length)];
                var plate1 = Instantiate(platePrefab, slots[0]);
                plate1.SetData(hero1, () => { });
                slots[0].gameObject.SetActive(true);
                for (var o = 1; o < slots.Length; o++)
                {
                    slots[o].gameObject.SetActive(false);
                }
                break;
        }
    }

    void CancelEncounter()
    {


    }
}
