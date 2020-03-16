using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsController : MonoBehaviour
{
    [Header("General Info")]
    [SerializeField] Image portrait;
    [SerializeField] Image rarityBorder;
    [SerializeField] TextMeshProUGUI charName;
    [SerializeField] TextMeshProUGUI charClass;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI totalLife;

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
    [SerializeField] TextMeshProUGUI hpAmount;
    [SerializeField] TextMeshProUGUI armorAmount;

    [Header("Dexterity SubStats")]
    [SerializeField] TextMeshProUGUI dodgeAmount;
    [SerializeField] TextMeshProUGUI speedAmount;

    [Header("Intelligence SubStats")]
    [SerializeField] TextMeshProUGUI magicDmgAmount;
    [SerializeField] TextMeshProUGUI magicCritAmount;
    [SerializeField] TextMeshProUGUI fireResAmount;
    [SerializeField] TextMeshProUGUI iceResAmount;
    [SerializeField] TextMeshProUGUI lightResAmount;

    [Header("Luck SubStats")]
    [SerializeField] TextMeshProUGUI critChanceAmount;

    [Header("Plugin Stats")]
    [SerializeField] TextMeshProUGUI physicalDamage;

    [Header("Skills")]
    [SerializeField] Image[] skills;

    [Header("Buttons")]
    [SerializeField] Button kickButton;

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

    [SerializeField] ItemController prefab_Item;
    [SerializeField] ItemInfoView itemInfo;

    Hero _viewedHero;
    Dictionary<ItemType, Transform> _slotsByType;

    private void Awake()
    {
        kickButton.onClick.AddListener(Kick);
    }

    void Kick()
    {
        ServiceRegistry.Characters.RemoveCharacter(_viewedHero);
    }

    public void SetHero(Hero hero)
    {
        _viewedHero = hero;

        SetGeneralInfo();
        SetStats();
        SetSubStats();
        SetSkillImages();
        SetPluginStats();

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

    public void OpenDetails()
    {
        CharacterDetailsView.SetCharacter(_viewedHero);
        ViewUtility.Show<CharacterDetailsView>();
    }

    private void SetSkillImages()
    {
        for (var i = 0; i < skills.Length; i++)
        {
            skills[i].sprite = _viewedHero.Skills[i].icon;
        }
    }

    private void SetGeneralInfo()
    {
        charName.text = _viewedHero.displayName;
        charClass.text = $"{_viewedHero.fightClass}";
        level.text = $"{_viewedHero.GetLevel()}";
        portrait.sprite = _viewedHero.GetPortrait();
        rarityBorder.color = Colors.ByRarity(_viewedHero.rarity);
        totalLife.text = $"{_viewedHero.MaxLife}";

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
    }

    void SetSubStats()
    {
        physDmgAmount.text = $"{_viewedHero.Sub.PhysicalDamage * 100f:N0}%";
        critPhysAmount.text = $"{_viewedHero.Sub.CriticalDamage * 100f:N0}%";
        hpAmount.text = $"{_viewedHero.Sub.Health}";
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
}
