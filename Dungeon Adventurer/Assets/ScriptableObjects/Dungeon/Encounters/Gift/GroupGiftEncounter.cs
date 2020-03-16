using UnityEngine;
using static EncounterResultController;

[CreateAssetMenu(fileName = "New GroupGift", menuName = "Encounter/GroupGift")]
public class GroupGiftEncounter : AGiftEncounter
{
    Hero[] _heroes;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        base.StartEncounter(encounterContent, heroes, level, rarity);

        _heroes = heroes;
        _controller.SetData(this, ApplyGifts, _heroes);
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
                result.Resolve(_heroes);
            }
        }
        FinishEncounter(new EncounterResult() { wasSucceded = true, endText = takenOption.normalResultText });
    }
}
