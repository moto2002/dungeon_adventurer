using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsView : View
{
    [SerializeField]
    private TextMeshProUGUI _charName;
    [SerializeField] TextMeshProUGUI charClass;

    [SerializeField] TextMeshProUGUI _strAmount;
    [SerializeField] TextMeshProUGUI _conAmount;
    [SerializeField] TextMeshProUGUI _dexAmount;
    [SerializeField] TextMeshProUGUI _intAmount;
    [SerializeField] TextMeshProUGUI _lckAmount;
 
    [SerializeField]
    private Button _detailsButton;

    private CharacterController _controller;

    public override void OnControllerChanged(Controller newController)
    {
        _controller = (CharacterController)newController;
        SetName();
        SetStats();
    }

    public void OpenDetails()
    {
        _detailsButton.interactable = false;
        SceneManagement._manager.ShowView("View_Details", _controller);
    }

    private void SetName()
    {
        _charName.text = _controller.attachedCharacter.name;
        charClass.text = _controller.attachedCharacter.fightClass.ToString();
    }

    private void SetStats()
    {
        _strAmount.text = _controller.attachedCharacter.main.str + "";
        _conAmount.text = _controller.attachedCharacter.main.con + "";
        _dexAmount.text = _controller.attachedCharacter.main.dex + "";
        _intAmount.text = _controller.attachedCharacter.main.intel + "";
        _lckAmount.text = _controller.attachedCharacter.main.lck + "";
    }
}
