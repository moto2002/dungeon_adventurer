using UnityEngine;

[CreateAssetMenu(fileName = "New Battle", menuName = "Encounter/Battle")]
public class BattleEncounter : AEncounter
{
    [SerializeField] BattleEncounterController controllerPrefab;

    BattleEncounterController _controller;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        _controller = Instantiate(controllerPrefab, encounterContent);
        _controller.gameObject.SetActive(true);
    }

    public override void FinishEncounter(EncounterResultController.EncounterResult result)
    {
        throw new System.NotImplementedException();
    }
}
