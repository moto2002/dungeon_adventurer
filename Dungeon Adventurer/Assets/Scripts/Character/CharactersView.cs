using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharactersView : View, CharactersService.OnCharactersChanged, InventoryService.OnInventoryChanged
{
    [Header("Controller")]
    [SerializeField] InventoryView inventoryView;

    [Header("Buttons")]

    [SerializeField] Button closeButton;

    [SerializeField] CharacterNavigation navigation;
    [SerializeField] List<ACharacterNavigationEntry> navigationEntries;
    [SerializeField] CharInfoController charInfoController;

    Hero _currentHero;

    protected override void Awake()
    {
        closeButton.onClick.AddListener(() => ViewUtility.ShowThenHide<CharacterOverviewView, CharactersView>());
        navigation.SetData(navigationEntries, navigationEntries[0]);
        charInfoController.SetData(OnHeroChanged);
    }

    void OnHeroChanged(Hero hero)
    {
        _currentHero = hero;
        navigationEntries.ForEach(entry => entry.RefreshHero(_currentHero));
        charInfoController.RefreshHero(_currentHero);
        inventoryView.Refresh(_currentHero);
    }

    public void SetCurrentHero(Hero hero)
    {
        OnHeroChanged(hero);
    }

    public void OnModelChanged(CharactersModel model)
    {
        charInfoController.RefreshHeroesModel(model);

        var hero = _currentHero != null ? model.Characters.FirstOrDefault(c => c.id == _currentHero.id) : null;
        if (hero == null)
        {
            if (model.Characters.Length > 0)
            {
                hero = model.Characters[0];
            }
            else
            {
                ViewUtility.HideThenShow<CharactersView, CharacterOverviewView>();
                return;
            }
        }
        OnHeroChanged(hero);
    }

    public void OnModelChanged(InventoryModel model)
    {
        inventoryView.RefreshInventory(model);
    }
}
