using UnityEngine;

public class DungeonView : View, DungeonService.OnDungeonChanged, DungeonService.OnBackPackChanged, CharactersService.OnCharactersChanged
{
    [SerializeField] DungeonController controller;

    public void SetBattleResult(BattleResult result)
    {
        controller.SetBattleResult(result);
    }

    public void OnModelChanged(DungeonModel model)
    {
        controller.RefreshDungeon(model);
    }

    public void OnModelChanged(BackpackModel model)
    {
        controller.RefreshBackpack(model);
    }

    public void OnModelChanged(CharactersModel characters)
    {
        controller.RefreshCharacters(characters);
    }
}
