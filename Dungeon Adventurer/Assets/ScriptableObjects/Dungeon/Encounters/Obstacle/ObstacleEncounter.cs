using UnityEngine;
using static EncounterResultController;

[CreateAssetMenu(fileName = "New Obstacle", menuName = "Encounter/Obstacle")]
public class ObstacleEncounter : AEncounter
{
    [SerializeField] ObstacleEncounterController controllerPrefab;

    [SerializeField] Option noReqOption;
    [SerializeField] Option firstSkillOption;
    [SerializeField] Option secondSkillOption;

    public string Description => mainDescription;
    public Option NoReqOption => noReqOption;
    public Option FirstSkillOption => firstSkillOption;
    public Option SecondSkillOption => secondSkillOption;

    ObstacleEncounterController _controller;
    Hero[] _heroes;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        _heroes = heroes;

        noReqOption.SetStats(level, rarity);
        firstSkillOption.SetStats(level, rarity);
        secondSkillOption.SetStats(level, rarity);

        _controller = Instantiate(controllerPrefab, encounterContent);
        _controller.SetData(this, heroes, RollOption);
        _controller.gameObject.SetActive(true);
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
                FinishEncounter(new EncounterResult() { wasSucceded = true, endText = clickedOption.successRate.SuccessText});
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
                result.Resolve(_heroes);
            }
        }
    }   

    public override void FinishEncounter(EncounterResult result)
    {
        Debug.Log("Obstacle finished");
        _controller.ShowResult(result);
    }
}
