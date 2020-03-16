using UnityEngine;
using static EncounterResultController;

[CreateAssetMenu(fileName = "New Trap", menuName = "Encounter/Trap")]
public class TrapEncounter : AEncounter
{
    [SerializeField] TrapEncounterController controllerPrefab;

    [SerializeField] Option firstOption;
    [SerializeField] Option secondOption;

    public Option FirstSkillOption => firstOption;
    public Option SecondSkillOption => secondOption;
    public string Description => mainDescription.Replace("_name_", _trappedHero.displayName);

    TrapEncounterController _controller;
    Hero _trappedHero;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        _trappedHero = heroes[Random.Range(0, heroes.Length)];

        firstOption.SetStats(level, rarity);
        secondOption.SetStats(level, rarity);

        _controller = Instantiate(controllerPrefab, encounterContent);
        _controller.SetData(this, _trappedHero, RollOption);
        _controller.gameObject.SetActive(true);
    }

    void RollOption(Option clickedOption)
    {
        OptionResult[] results = null;
        if (clickedOption.successRate.NeedRoll)
        {
            var heroValue = _trappedHero.Main.GetValue(clickedOption.successRate.stat);
            
            var successPercent = Mathf.Min((Mathf.Max(heroValue - clickedOption.successRate.MinValue, 0f) / clickedOption.successRate.Difference), 0.95f);
            var roll = Random.value;
            if (roll < successPercent)
            {
                Debug.Log("Succeeded Option with a roll of " + roll + $" ({successPercent})");
                FinishEncounter(new EncounterResult() { wasSucceded = true, endText = clickedOption.successRate.SuccessText });
            } else
            {
                Debug.Log("Failed Option with a roll of " + roll + $" ({successPercent})");
                results = clickedOption.results;
                FinishEncounter(new EncounterResult() { wasSucceded = false, endText = clickedOption.normalResultText });
            }
        } else
        {
            results = clickedOption.results;
        }

        if (results != null)
        {
            foreach (var result in results)
            {
                result.Resolve(_trappedHero);
            }
        }
    }

    public override void FinishEncounter(EncounterResult result)
    {
        Debug.Log("Trap finished");
        _controller.ShowResult(result);
    }
}
