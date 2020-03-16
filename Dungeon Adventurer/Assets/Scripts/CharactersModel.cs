using System.Linq;
using UnityEngine;

public class CharactersModel
{
    public Hero[] Characters => _characters.characters;
    public CharacterData Data => _characters;

    CharacterData _characters;

    public CharactersModel(CharacterData characters)
    {
        _characters = characters;
        if (_characters == null || _characters.characters == null)
        {
            _characters = new CharacterData();
            _characters.characters = new Hero[0];
        } else {
            foreach (var character in _characters.characters)
            {
                character.ApplyItems();
                character.ApplySkillInfos();
            }
        } 
    }

    public Hero GetHeroById(int id)
    {
        return _characters.characters.FirstOrDefault(e => e.id == id);
    }
}
