using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharInfoController : ACharacterNavigationEntry
{
    [Header("Buttons")]
    [SerializeField] Button kickButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button lastButton;

    [Header("General Info")]
    [SerializeField] Image portrait;
    [SerializeField] Image rarityBorder;
    [SerializeField] TextMeshProUGUI charName;
    [SerializeField] TextMeshProUGUI charClass;
    [SerializeField] TextMeshProUGUI level;

    [Header("Character Slots")]
    [SerializeField] Transform amuletSlot;
    [SerializeField] Transform helmetSlot;
    [SerializeField] Transform chestSlot;
    [SerializeField] Transform shoulderSlot;
    [SerializeField] Transform beltSlot;
    [SerializeField] Transform pantsSlot;
    [SerializeField] Transform glovesSlot;
    [SerializeField] Transform bracersSlot;
    [SerializeField] Transform bootsSlot;
    [SerializeField] Transform ringSlot;
    [SerializeField] Transform weaponSlot;

    [Header("Items")]
    [SerializeField] ItemController prefab_Item;
    [SerializeField] ItemInfoView itemInfo;

    Hero _viewedHero;
    Dictionary<ItemType, Transform> _slotsByType;
    Action<Hero> _callback;
    Hero[] _heroes;

    protected override void Awake()
    {
        kickButton.onClick.AddListener(Kick);
        nextButton.onClick.AddListener(Next);
        lastButton.onClick.AddListener(Last);
    }

    void Next()
    {
        Loop(_heroes);
    }

    void Last()
    {
        Loop(_heroes.Reverse().ToArray());
    }

    void Loop(Hero[] heroes)
    {
        var returnHero = heroes.LastOrDefault() == _viewedHero
            ? heroes.First()
            : heroes
                .SkipWhile(hero => hero != _viewedHero)
                .Skip(1)
                .First();
        _callback?.Invoke(returnHero);
    }

    void Kick()
    {
        ServiceRegistry.Characters.RemoveCharacter(_viewedHero);
    }

    public void SetData(Action<Hero> callback)
    {
        _callback = callback;
    }

    public void RefreshHeroesModel(CharactersModel heroes)
    {
        _heroes = heroes.Characters;
    }

    public void RefreshHeroes(Hero[] heroes)
    {
        _heroes = heroes;
    }

    public override void RefreshHero(Hero hero)
    {
        _viewedHero = hero;

        SetGeneralInfo();

        RemoveCharacterSlotItems();

        var list = _viewedHero.GetAppliedItems();
        if (list == null) return;

        //TODO: Rework Brute Force enable Default Slot image
        foreach (var slot in _slotsByType.Values)
        {
            slot.GetChild(0).GetComponent<Image>().enabled = true;
        }
        foreach (var item in list)
        {
            var i = Instantiate(prefab_Item, _slotsByType[item.Key]);
            _slotsByType[item.Key].GetChild(0).GetComponent<Image>().enabled = false;
            i.SetData(item.Value, delegate { itemInfo.SetData(_viewedHero, item.Value, true); });
        }
    }

    void RemoveCharacterSlotItems()
    {
        if (_slotsByType == null)
        {
            _slotsByType = new Dictionary<ItemType, Transform>() {
                            {ItemType.Amulet, amuletSlot },
                            {ItemType.Helmet, helmetSlot },
                            {ItemType.Chest, chestSlot },
                            {ItemType.Shoulder, shoulderSlot },
                            {ItemType.Belts, beltSlot },
                            {ItemType.Gloves, glovesSlot },
                            {ItemType.Boots, bootsSlot },
                            {ItemType.Ring, ringSlot },
                            {ItemType.Weapon, weaponSlot },
                            {ItemType.Pants, pantsSlot },
                            {ItemType.Bracers, bracersSlot }
                            };
        }

        foreach (var trans in _slotsByType.Values)
        {
            if (trans.childCount <= 1) continue;

            Destroy(trans.GetChild(1).gameObject);
        }
    }

    void SetGeneralInfo()
    {
        charName.text = _viewedHero.displayName;
        charClass.text = $"{_viewedHero.fightClass}";
        level.text = $"{_viewedHero.GetLevel()}";
        portrait.sprite = _viewedHero.GetPortrait();
        rarityBorder.color = Colors.ByRarity(_viewedHero.rarity);
    }
}
