using UnityEngine;
using static EncounterResultController;

public abstract class AEncounter : ScriptableObject
{
    [Header("Abstract Implementation")]
    [SerializeField] protected bool optional;

    [Header("Story")]
    [SerializeField] protected string mainDescription;

    public bool Optional => optional;

    public abstract void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity);
    public abstract void FinishEncounter(EncounterResult result);

}
