using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsController : ACharacterNavigationEntry
{
    [Header("Main Stats")]
    [SerializeField] TextMeshProUGUI strAmount;
    [SerializeField] TextMeshProUGUI conAmount;
    [SerializeField] TextMeshProUGUI dexAmount;
    [SerializeField] TextMeshProUGUI intAmount;
    [SerializeField] TextMeshProUGUI lckAmount;

    [Header("Averages")]
    [SerializeField] TextMeshProUGUI dmgAverage;
    [SerializeField] TextMeshProUGUI toughAverage;
    [SerializeField] TextMeshProUGUI utilityAverage;

    [Header("Strength SubStats")]
    [SerializeField] TextMeshProUGUI physDmgAmount;
    [SerializeField] TextMeshProUGUI critPhysAmount;

    [Header("Constitution SubStats")]
    [SerializeField] TextMeshProUGUI healthAmount;
    [SerializeField] TextMeshProUGUI healthRegenAmount;
    [SerializeField] TextMeshProUGUI armorAmount;

    [Header("Dexterity SubStats")]
    [SerializeField] TextMeshProUGUI dodgeAmount;
    [SerializeField] TextMeshProUGUI speedAmount;

    [Header("Intelligence SubStats")]
    [SerializeField] TextMeshProUGUI manaAmount;
    [SerializeField] TextMeshProUGUI manaRegenAmount;
    [SerializeField] TextMeshProUGUI magicDmgAmount;
    [SerializeField] TextMeshProUGUI magicCritAmount;
    [SerializeField] TextMeshProUGUI fireResAmount;
    [SerializeField] TextMeshProUGUI iceResAmount;
    [SerializeField] TextMeshProUGUI lightResAmount;

    [Header("Luck SubStats")]
    [SerializeField] TextMeshProUGUI critChanceAmount;

    [Header("Plugin Stats")]
    [SerializeField] TextMeshProUGUI physicalDamage;

    [SerializeField] TextMeshProUGUI totalLife;
    [SerializeField] TextMeshProUGUI totalMana;

    [SerializeField] TextMeshProUGUI xpAmount;
    [SerializeField] Slider xpSlider;
    [SerializeField] Button levelUpButton;

    Hero _viewedHero;

    protected override void Awake()
    {
        levelUpButton.onClick.AddListener(RankUp);
    }

    public override void RefreshHero(Hero hero)
    {
        _viewedHero = hero;

        SetAverages();
        SetStats();
        SetSubStats();
        SetPluginStats();
        SetExperience();
    }

    void SetAverages()
    {
        dmgAverage.text = $"{_viewedHero.AverageDamage}";
        toughAverage.text = $"{_viewedHero.AverageToughness}";
        utilityAverage.text = $"{_viewedHero.AverageUtility}";
    }

    private void SetStats()
    {
        strAmount.text = $"{_viewedHero.Main.str}";
        conAmount.text = $"{_viewedHero.Main.con}";
        dexAmount.text = $"{_viewedHero.Main.dex}";
        intAmount.text = $"{_viewedHero.Main.intel}";
        lckAmount.text = $"{_viewedHero.Main.lck}";

        totalLife.text = $"{_viewedHero.MaxLife}";
        totalMana.text = $"{_viewedHero.MaxMana}";
    }

    void SetSubStats()
    {
        physDmgAmount.text = $"{_viewedHero.Sub.PhysicalDamage * 100f:N0}%";
        critPhysAmount.text = $"{_viewedHero.Sub.CriticalDamage * 100f:N0}%";
        healthAmount.text = $"{_viewedHero.Sub.Health}";
        manaAmount.text = $"{_viewedHero.Sub.Mana}";
        manaRegenAmount.text = $"{_viewedHero.Sub.ManaRegen}";
        healthRegenAmount.text = $"{_viewedHero.Sub.HealthRegen}";
        armorAmount.text = $"{_viewedHero.Sub.Armor * 100f:N0}";
        dodgeAmount.text = $"{_viewedHero.Sub.Dodge * 100f:N0}%";
        speedAmount.text = $"{_viewedHero.Sub.Speed:N0}";
        magicDmgAmount.text = $"{_viewedHero.Sub.MagicDamage * 100f:N0}%";
        magicCritAmount.text = $"{_viewedHero.Sub.MagicCritical * 100f:N0}%";
        fireResAmount.text = $"{_viewedHero.Sub.FireResistance * 100f:N0}%";
        iceResAmount.text = $"{_viewedHero.Sub.IceResistance * 100f:N0}%";
        lightResAmount.text = $"{_viewedHero.Sub.LightningResistance * 100f:N0}%";
        critChanceAmount.text = $"{ _viewedHero.Sub.CriticalChance * 100f:N0}%";
    }

    void SetPluginStats()
    {
        physicalDamage.text = $"{_viewedHero.MinPhysicalDamage} - {_viewedHero.MaxPhysicalDamage}";
    }

    void SetExperience()
    {
        var cur = _viewedHero.level.currentExp;
        var max = CharactersService.ExperienceByLevel[_viewedHero.level.currentLevel];
        xpAmount.text = $"{cur} / {max}";
        xpSlider.maxValue = max;
        xpSlider.value = cur;
        levelUpButton.interactable = _viewedHero.level.CanBeLeveled;
    }

    void RankUp()
    {
        ServiceRegistry.Characters.RankUp(_viewedHero.id);
    }
}
