using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleOrderView : View
{
    CharactersController _controller;
    [SerializeField] Transform scrollContent;
    [SerializeField] Transform compContent;
    [SerializeField] BattleOrderSlot prefabSlot;
    [SerializeField] CompSlot prefabCompSlots;
    [SerializeField] DungeonMenu dungeonMenu;

    public static BattleOrderView _instance;

    CompSlot[] slots;

    BattleOrderSlot prefab;

    public delegate void SlotEventHandler();
    public static event SlotEventHandler OnSlotSelected;

    Hero _selectedCharacter;

    public override void OnControllerChanged(Controller newController)
    {
        _instance = this;
        _controller = (CharactersController)newController;
        Init();
    }

    void Init()
    {
        prefab = prefabSlot;
        dungeonMenu.Init();
        InstantiateCharacters();
        InstantiateCompSlots();
    }

    void InstantiateCharacters()
    {
        foreach (var character in _controller._model.characters)
        {
            var slot = Instantiate(prefabSlot, scrollContent);
            slot.SetData(character);
        }
    }

    void InstantiateCompSlots()
    {
        slots = new CompSlot[9];
        List<Hero> chars = new List<Hero>(DataHolder._data.CharData.characters);
        for (int i = 0; i < 9; i++)
        {
            var character = chars.Find(e => e.position == i && e.position != 0);
            var campSlot = Instantiate(prefabCompSlots, compContent);
            campSlot.SetData(i);
            slots[i] = campSlot;

            if (character != null)
            {
                Debug.Log("Position:" + character.position);
                var battleOrderSlot = Instantiate(prefab);
                battleOrderSlot.SetData(character);
                battleOrderSlot.GetComponent<Image>().raycastTarget = false;
                campSlot.ApplyBattleOrderSlot(battleOrderSlot);
                characterPositions.Add(character, i);
            }
        }
    }

    public void CharacterSelected(Hero character)
    {
        _selectedCharacter = character;
        OnSlotSelected();
    }

    Dictionary<Hero, int> characterPositions = new Dictionary<Hero, int>();

    public BattleOrderSlot SlotSelected(int pos)
    {
        if (_selectedCharacter != null)
        {
            if (!characterPositions.ContainsKey(_selectedCharacter))
            {
                CheckPositionValues(pos);
                characterPositions.Add(_selectedCharacter, pos);
                DataHolder._data.SetCharacterPosition(_selectedCharacter.id, pos);
            }
            else
            {
                CheckPositionValues(pos);
                slots[characterPositions[_selectedCharacter]].CleanTransform();
                characterPositions[_selectedCharacter] = pos;
                DataHolder._data.SetCharacterPosition(_selectedCharacter.id, pos);
            }

            var s = Instantiate(prefab);
            s.SetData(_selectedCharacter);
            s.GetComponent<Image>().raycastTarget = false;
            return s;
        }
        return null;
    }

    void CheckPositionValues(int pos) {

        if (characterPositions.ContainsValue(pos))
        {
            foreach (var item in characterPositions)
            {
                if (item.Value == pos)
                {
                    DataHolder._data.SetCharacterPosition(item.Key.id, 10);
                    break;
                }
            }
        }
    }
}
