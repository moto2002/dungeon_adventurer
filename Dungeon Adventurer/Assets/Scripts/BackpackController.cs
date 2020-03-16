using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackController : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] CharacterBackpack prefab;
    [SerializeField] ItemInfoView itemInfo;
    [SerializeField] Button typeButton;
    [SerializeField] Button priceButton;
    [SerializeField] Button rarityButton;

    List<CharacterBackpack> _createdBackpacks = new List<CharacterBackpack>();
    Hero[] _enteredHeroes;
    BackpackModel _backpack;
    Hero _currentHero;

    void Awake()
    {
        priceButton.onClick.AddListener(() => _createdBackpacks.ForEach(bp => bp.SetFilter(1)));
        rarityButton.onClick.AddListener(() => _createdBackpacks.ForEach(bp => bp.SetFilter(2)));
        typeButton.onClick.AddListener(() => _createdBackpacks.ForEach(bp => bp.SetFilter(3)));
    }

    public void RefreshCurrentHero(Hero currentHero)
    {
        _currentHero = currentHero;
        if (_createdBackpacks == null) return;
        _createdBackpacks.ForEach(bp => bp.Refresh(currentHero, itemInfo));
    }

    public void RefreshHeroes(Hero[] heroes)
    {
        _enteredHeroes = heroes;
        RefreshSlots();
    }

    public void RefreshBackpack(BackpackModel backpack)
    {
        _backpack = backpack;
        RefreshSlots();
    }

    void RefreshSlots()
    {
        if (_backpack == null || _enteredHeroes == null) return;

        foreach (Transform trans in container)
        {
            Destroy(trans.gameObject);
        }

        _createdBackpacks.Clear();

        var offset = -40f;
        for (var i = 0; i < _enteredHeroes.Length; i++)
        {
            var bp = Instantiate(prefab, container);
            bp.transform.localPosition = new Vector2(0, offset);
            var hero = _enteredHeroes[i];
            bp.RefreshItems(hero, _backpack.PossibleSlots[hero.id], _backpack.InventoryItems[hero.id]);
            var height = 70 + 60 * ((_backpack.PossibleSlots[hero.id] - 1) / 8);
            offset -= height + 45f;
            _createdBackpacks.Add(bp);
        }

        container.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, offset * -1);

        if (_currentHero == null) return;
        _createdBackpacks.ForEach(bp => bp.Refresh(_currentHero, itemInfo));
    }
}