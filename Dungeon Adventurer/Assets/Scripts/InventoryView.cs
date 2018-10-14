using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryView : View {

    private CharactersController _controller;

    [SerializeField]
    private CharacterStatsView _statsView;

    public override void OnControllerChanged(Controller newController)
    {
        _controller = (CharactersController)newController;
        _statsView.Controller = new CharacterController(_controller._model.characters[0]);
    }

    public void ChangeCharacter(int id)
    {
        _statsView.Controller = new CharacterController(_controller.ChangeCharacterView(id));
    }


}
