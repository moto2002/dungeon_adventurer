using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static EncounterResultController;

[CreateAssetMenu(fileName = "New Generic", menuName = "Encounter/Generic")]
public class GenericEncounter : AEncounter
{
    [SerializeField] List<Option> options;
    [SerializeField] GenericEncounterController controllerPrefab;

    public string Description => mainDescription;
    public List<Option> Options => options;

    GenericEncounterController _controller;

    Hero[] _heroes;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        _heroes = heroes;

        options.ForEach(option => option.SetStats(level, rarity));

        _controller = Instantiate(controllerPrefab, encounterContent);
        _controller.SetData(this, _heroes, RollOption);
        _controller.gameObject.SetActive(true);
    }
    public override void FinishEncounter(EncounterResult result)
    {
        Debug.Log("Generic finished");
        _controller.ShowResult(result);
    }

    void RollOption(Option clickedOption)
    {
        OptionResult[] results = null;
        if (clickedOption.successRate.NeedRoll)
        {
            var heroValue = 0;
            foreach (var hero in _heroes)
            {
                heroValue += hero.Main.GetValue(clickedOption.successRate.stat);
            }

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
            FinishEncounter(new EncounterResult() { wasSucceded = true, endText = clickedOption.normalResultText });
        }

        if (results != null)
        {
            foreach (var result in results)
            {
                result.Resolve(_heroes);
            }
        }
    }

    public void StartFight()
    {
        Debug.Log("Start Fight");
        ServiceRegistry.Dungeon.StartBattle();
    }

    public void ApplyGift()
    {

    }

    public void Apply()
    {

    }
}
