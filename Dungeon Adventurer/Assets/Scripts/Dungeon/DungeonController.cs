using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DungeonController : UIBehaviour
{
    [SerializeField] Button leaveButton;
    [SerializeField] Button charsButton;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject characterWindow;
    [SerializeField] MapController mapController;
    [SerializeField] BackpackController backpackController;

    [SerializeField] CharacterNavigation navigation;
    [SerializeField] List<ACharacterNavigationEntry> navigationEntries;
    [SerializeField] CharInfoController charInfoController;
    [SerializeField] BattleRewardController rewardController;
    [SerializeField] Transform battleRoom;

    DungeonModel _dungeon;
    List<int> _enteredIds;
    Hero[] _allHeroes;
    Hero[] _enteredHeroes;
    Hero _currentHero;

    protected override void Awake()
    {
        leaveButton.onClick.AddListener(Leave);
        charsButton.onClick.AddListener(() => characterWindow.SetActive(true));
        closeButton.onClick.AddListener(() => characterWindow.SetActive(false));
        mapController.SetCallback(OnRoomChanged);

        charInfoController.SetData(OnHeroChanged);
        navigation.SetData(navigationEntries, navigationEntries[0]);
    }

    public void SetBattleResult(BattleResult result)
    {
        rewardController.SetBattleResult(result);
    }

    public void Leave()
    {
        ViewUtility.ShowThenHide<StartView, DungeonView>();
    }

    void MaybeRefreshCharacters()
    {
        if (_allHeroes == null || _enteredIds == null) return;

        _enteredHeroes = _allHeroes.Where(hero => _enteredIds.Contains(hero.id)).ToArray();
        var fakeHero = _currentHero == null ? _enteredHeroes[0] : _enteredHeroes.First(hero => hero.id == _currentHero.id);

        charInfoController.RefreshHeroes(_enteredHeroes);
        backpackController.RefreshHeroes(_enteredHeroes);

        OnHeroChanged(fakeHero);
    }

    void OnHeroChanged(Hero hero)
    {
        _currentHero = hero;

        navigationEntries.ForEach(entry => entry.RefreshHero(_currentHero));
        charInfoController.RefreshHero(_currentHero);
        backpackController.RefreshCurrentHero(_currentHero);
    }

    void OnRoomChanged(MapRoom room)
    {
        var encounterData = room.Encounter;
        if (encounterData == null) return;

        Debug.Log("Encounter Level = " + encounterData.level + " and Rarity=" + encounterData.rarity.ToString());
        encounterData.encounter.StartEncounter(battleRoom, _enteredHeroes, encounterData.level, encounterData.rarity);
    }

    public void RefreshDungeon(DungeonModel model)
    {
        mapController.CreateMap(model.Rooms);
        _enteredIds = model.EnteredHeroes.Select(hero => hero.id).ToList();
        MaybeRefreshCharacters();
    }

    public void RefreshBackpack(BackpackModel model)
    {
        backpackController.RefreshBackpack(model);
        rewardController.RefreshOpenSlots(model.OpenSlots());
    }

    public void RefreshCharacters(CharactersModel model)
    {
        _allHeroes = model.Characters;
        MaybeRefreshCharacters();
    }
}
