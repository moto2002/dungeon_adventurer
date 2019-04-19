using UnityEngine;

public class CharactersController : Controller
{
    public CharacterData _model;

    private int _selectedCharacter = 0;

    public CharactersController(CharacterData Model)
    {
        this._model = Model;
    }

    public Hero ChangeCharacterView(int id)
    {
        Debug.Log(id + " and " + _model.characters.Length);
        return _model.characters[id];
    }
}

public class CharacterController : Controller
{
    public Hero attachedCharacter;

    public CharacterController(Hero character)
    {
        attachedCharacter = character;
    }
}
