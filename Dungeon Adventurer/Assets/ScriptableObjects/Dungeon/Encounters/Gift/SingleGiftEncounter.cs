using UnityEngine;
using static EncounterResultController;

[CreateAssetMenu(fileName = "New SingleGift", menuName = "Encounter/SingleGift")]
public class SingleGiftEncounter : AGiftEncounter
{
    Hero _hero;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        base.StartEncounter(encounterContent, heroes, level, rarity);

        _hero = heroes[Random.Range(0, heroes.Length)];
        _controller.SetData(this, ApplyGifts, _hero);
        _controller.gameObject.SetActive(true);
    }

    public override void ApplyGifts(Option takenOption)
    {
        OptionResult[] results = null;
        results = takenOption.results;

        if (results != null)
        {
            foreach (var result in results)
            {
                result.Resolve(_hero);
            }
        }
        FinishEncounter(new EncounterResult() { wasSucceded = true, endText = takenOption.normalResultText });
    }
}
