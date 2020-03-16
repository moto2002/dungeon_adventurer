using UnityEngine;
using static EncounterResultController;

public abstract class AGiftEncounter : AEncounter
{
    [SerializeField] GiftEncounterController controllerPrefab;

    [SerializeField] Option takeOption;

    public string Description => mainDescription;
    public Option TakeOption => takeOption;

    protected GiftEncounterController _controller;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        takeOption.SetStats(level, rarity);

        _controller = Instantiate(controllerPrefab, encounterContent);
    }

    public abstract void ApplyGifts(Option takenOption);

    public override void FinishEncounter(EncounterResult result)
    {
        Debug.Log("Gift taken");
        _controller.ShowResult(result);
    }
}
