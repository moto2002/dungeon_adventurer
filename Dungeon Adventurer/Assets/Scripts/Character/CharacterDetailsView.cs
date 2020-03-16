using TMPro;
using UnityEngine;

public class CharacterDetailsView : View
{
    [SerializeField] TextMeshProUGUI _charName;

    //Strength
    [SerializeField] TextMeshProUGUI _physDmgAmount, _critPhysAmount;

    //Constitution
    [SerializeField] TextMeshProUGUI _hpAmount, _armorAmount;

    //Dexterity
    [SerializeField] TextMeshProUGUI _dodgeAmount, _speedAmount;

    //Intelligence
    [SerializeField] TextMeshProUGUI _magicDmgAmount, _magicCritAmount, _fireResAmount, _iceResAmount, _lightResAmount;

    //Luck
    [SerializeField] TextMeshProUGUI _critChanceAmount;

    static Hero _hero;

    private void OnEnable()
    {
        SetSubStats();
        SetName();
    }

    public static void SetCharacter(Hero hero)
    {
        _hero = hero;
    }

    void SetName()
    {
        _charName.text = _hero.displayName;
    }

    void SetSubStats()
    {
        _physDmgAmount.text = _hero.Sub.physDmg + "";
        _critPhysAmount.text = _hero.Sub.critDmg + "";
        _hpAmount.text = _hero.Sub.hp + "";
        _armorAmount.text = _hero.Sub.armor + "";
        _dodgeAmount.text = _hero.Sub.dodge + "";
        _speedAmount.text = _hero.Sub.speed + "";
        _magicDmgAmount.text = _hero.Sub.magicDmg + "";
        _magicCritAmount.text = _hero.Sub.critMagic + "";
        _fireResAmount.text = _hero.Sub.fireRes + "";
        _iceResAmount.text = _hero.Sub.iceRes + "";
        _lightResAmount.text = _hero.Sub.lightRes + "";
        _critChanceAmount.text = _hero.Sub.critChance + "";
    }
}
