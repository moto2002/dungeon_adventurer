using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersController : Controller{

    public CharacterData _model;

    private int _selectedCharacter = 0;

    public CharactersController(CharacterData Model)
    {
        this._model = Model;
    }


    public Character ChangeCharacterView(int id)
    {
        return _model.characters[id];
    }
}

public class CharacterController : Controller
{
    public Character attachedCharacter;

    public CharacterController(Character character)
    {
        attachedCharacter = character;
    }
}
