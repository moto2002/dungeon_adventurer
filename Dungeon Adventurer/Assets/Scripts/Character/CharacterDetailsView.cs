using TMPro;
using UnityEngine;

public class CharacterDetailsView : View
{

    [SerializeField]
    TextMeshProUGUI _charName;

    //Strength
    [SerializeField]
    TextMeshProUGUI _physDmgAmount, _critPhysAmount;

    //Constitution
    [SerializeField]
    TextMeshProUGUI _hpAmount, _armorAmount;

    //Dexterity
    [SerializeField]
    TextMeshProUGUI _dodgeAmount, _speedAmount;

    //Intelligence
    [SerializeField]
    TextMeshProUGUI _magicDmgAmount, _magicCritAmount, _fireResAmount, _iceResAmount, _lightResAmount;

    //Luck
    [SerializeField]
    private TextMeshProUGUI _critChanceAmount;
    private CharacterController _controller;

    public override void OnControllerChanged(Controller newController)
    {
        _controller = (CharacterController)newController;
        SetSubStats();
    }

    private void SetName()
    {
        _charName.text = _controller.attachedCharacter.name;

    }

    private void SetSubStats()
    {
        _physDmgAmount.text = _controller.attachedCharacter.sub.physDmg + "";
        _critPhysAmount.text = _controller.attachedCharacter.sub.critDmg + "";
        _hpAmount.text = _controller.attachedCharacter.sub.hp + "";
        _armorAmount.text = _controller.attachedCharacter.sub.armor + "";
        _dodgeAmount.text = _controller.attachedCharacter.sub.dodge + "";
        _speedAmount.text = _controller.attachedCharacter.sub.speed + "";
        _magicDmgAmount.text = _controller.attachedCharacter.sub.magicDmg + "";
        _magicCritAmount.text = _controller.attachedCharacter.sub.critMagic + "";
        _fireResAmount.text = _controller.attachedCharacter.sub.fireRes + "";
        _iceResAmount.text = _controller.attachedCharacter.sub.iceRes + "";
        _lightResAmount.text = _controller.attachedCharacter.sub.lightRes + "";
        _critChanceAmount.text = _controller.attachedCharacter.sub.critChance + "";
    }
}
