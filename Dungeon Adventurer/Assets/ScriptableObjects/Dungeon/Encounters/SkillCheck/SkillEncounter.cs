using System.Collections.Generic;
using UnityEngine;
using static EncounterResultController;

[CreateAssetMenu(fileName = "New SkillEncounter", menuName = "Encounter/SkillEncounter")]
public class SkillEncounter : AEncounter
{
    const string NameReplace = "_name_";
    static readonly Dictionary<Rarity, int> _requiredValuePerLevel = new Dictionary<Rarity, int>()
    {
        { Rarity.Common, 10},
        { Rarity.Uncommon, 15},
        { Rarity.Magic, 20},
        { Rarity.Rare, 25},
        { Rarity.Epic, 30},
        { Rarity.Legendary, 40},
        { Rarity.Unique, 50},
    };

    [Header("Type")]
    [SerializeField] CheckType checkType;
    [SerializeField] MainStat mainStat;

    [SerializeField] SkillEncounterController controllerPrefab;

    public string Description(Hero hero = null) => checkType == CheckType.Group ? mainDescription : mainDescription.Replace(NameReplace, hero.displayName);
    public CheckType CheckType => checkType;
    public MainStat MainStat => mainStat;

    Rarity _rarity;
    Hero[] _heroes;
    Hero _choosenHero;
    int _requiredValue;
    SkillEncounterController _controller;

    public override void StartEncounter(Transform encounterContent, Hero[] heroes, int level, Rarity rarity)
    {
        _controller = Instantiate(controllerPrefab, encounterContent);
        _controller.SetData(this, heroes, level, StartCheck);
        _controller.gameObject.SetActive(true);

        _heroes = heroes;
        _choosenHero = _heroes[Random.Range(0, _heroes.Length)];
        _rarity = rarity;
        _requiredValue = _requiredValuePerLevel[_rarity] * level * (checkType == CheckType.Group ? _heroes.Length : 1);
    }

    void StartCheck()
    {
        var checkValue = 0;
        if (CheckType == CheckType.Group)
        {
            foreach (var hero in _heroes)
            {
                checkValue += hero.Main.GetValue(MainStat);
            }
        } else
        {
            checkValue += _choosenHero.Main.GetValue(MainStat);
        }

        if (checkValue >= _requiredValue)
        {
            Succeed();
        } else
        {
            Fail();
        }
    }

    public void Succeed()
    {
    }

    public void Fail()
    {
    }

    public override void FinishEncounter(EncounterResult result)
    {
    }

}

public enum CheckType
{
    Single,
    Group
}

