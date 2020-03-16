using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaverneCharacter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI clas;
    [SerializeField] TextMeshProUGUI level;

    [SerializeField] TextMeshProUGUI str;
    [SerializeField] TextMeshProUGUI con;
    [SerializeField] TextMeshProUGUI dex;
    [SerializeField] TextMeshProUGUI intel;
    [SerializeField] TextMeshProUGUI lck;

    [SerializeField] Button recruitButton;
    [SerializeField] TextMeshProUGUI life;
    [SerializeField] TextMeshProUGUI mana;

    [SerializeField] Image border;
    [SerializeField] Image portrait;

    Hero _appliedCharacter;
    double _price;

    Dictionary<Rarity, double> costPerRarity = new Dictionary<Rarity, double>
    {
        {Rarity.Common, 1 },
        {Rarity.Uncommon, 2 },
        {Rarity.Magic, 5 },
        {Rarity.Rare, 15 },
        {Rarity.Epic, 45 },
        {Rarity.Legendary, 150 },
        {Rarity.Unique, 420 },
    };

    public void SetData(Hero hero, CurrencyModel _model) {

        _appliedCharacter = hero;

        name.text = _appliedCharacter.displayName;
        clas.text = _appliedCharacter.fightClass.ToString();
        level.text = "" + 1;

        str.text = $"{_appliedCharacter.Main.str}";
        con.text = $"{_appliedCharacter.Main.con}";
        dex.text = $"{_appliedCharacter.Main.dex}";
        intel.text = $"{_appliedCharacter.Main.intel}";
        lck.text = $"{_appliedCharacter.Main.lck}";

        life.text = $"{_appliedCharacter.MaxLife}";
        mana.text = $"{_appliedCharacter.MaxMana}";

        border.color = Colors.ByRarity(_appliedCharacter.rarity);
        portrait.sprite = DataHolder._data.raceImages[(int)_appliedCharacter.race];

        _price = _appliedCharacter.GetHeroStrength() * costPerRarity[_appliedCharacter.rarity];
        recruitButton.onClick.AddListener(() => { CallBack(_appliedCharacter); });
        recruitButton.interactable = _price <= _model.GetCurrency(Currency.Coins);
    }

    void Refresh(Currency cur, double value, int change)
    {
        if (cur != Currency.Coins) return;
        recruitButton.interactable = _price <= value;
    }

    void CallBack(Hero data)
    {
        if (!ServiceRegistry.Currency.TryPurchase(Currency.Coins, _price)) return;
        ServiceRegistry.Characters.AddCharacter(_appliedCharacter);
        Destroy(gameObject);
    }
}
