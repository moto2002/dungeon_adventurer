using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterStatsView : View {

    [SerializeField]
    private TextMeshProUGUI _charName;

    private CharacterController _controller;

    public override void OnControllerChanged(Controller newController)
    {
        _controller = (CharacterController) newController;
        SetName();
    }

    private void SetName()
    {
        _charName.text = _controller.attachedCharacter.name;

    }
}
