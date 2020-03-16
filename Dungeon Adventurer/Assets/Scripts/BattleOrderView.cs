using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleOrderView : View, CharactersService.OnCharactersChanged
{
    [SerializeField] Transform scrollContent;
    [SerializeField] Transform compContent;
    [SerializeField] BattleOrderSlot prefabSlot;
    [SerializeField] CompSlot prefabCompSlots;
    [SerializeField] DungeonMenu dungeonMenu;
    [SerializeField] Button closeButton;
    [SerializeField] CharacterInfo characterInfo;

    public static BattleOrderView _instance;
    CharactersModel _heroesModel;
    CompSlot[] _slots;
    Hero _selectedCharacter;

    public delegate void SlotEventHandler();
    public static event SlotEventHandler OnSlotSelected;

    protected override void Awake()
    {
        _instance = this;
        closeButton.onClick.AddListener(Close);
        dungeonMenu.Init();
    }

    void Init()
    {
        ClearAll();
        InstantiateCharacters();
        InstantiateCompSlots();
    }

    void InstantiateCharacters()
    {
        foreach (var character in _heroesModel.Characters)
        {
            var slot = Instantiate(prefabSlot, scrollContent);
            slot.SetData(character);
        }
    }

    void ClearAll()
    {
        characterPositions = new Dictionary<Hero, int>();

        foreach (Transform trans in scrollContent)
        {
            Destroy(trans.gameObject);
        }

        foreach (Transform trans in compContent)
        {
            Destroy(trans.gameObject);
        }
    }

    void InstantiateCompSlots()
    {
        _slots = new CompSlot[9];
        List<Hero> chars = new List<Hero>(_heroesModel.Characters);
        for (int i = 0; i < 9; i++)
        {
            var character = chars.FirstOrDefault(e => e.position == i);
            var campSlot = Instantiate(prefabCompSlots, compContent);
            campSlot.SetData(i);
            _slots[i] = campSlot;

            if (character != null)
            {
                var battleOrderSlot = Instantiate(prefabSlot);
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
        characterInfo.SetData(_selectedCharacter);
        AdjustFieldColors();
        OnSlotSelected();
    }

    void AdjustFieldColors()
    {
        for (var i = 0; i < _slots.Length; i++)
        {
            var posSkills = 0;
            for (var u = 0; u < 4; u++)
            {
                if (_selectedCharacter.Skills[u].possiblePositions[i])
                    posSkills++;
            }
            _slots[i].SetColor(posSkills);
        }
    }

    Dictionary<Hero, int> characterPositions;

    public BattleOrderSlot SlotSelected(int pos)
    {
        if (_selectedCharacter != null)
        {
            if (!characterPositions.ContainsKey(_selectedCharacter))
            {
                CheckPositionValues(pos);
                characterPositions.Add(_selectedCharacter, pos);
                ServiceRegistry.Characters.SetCharacterPosition(_selectedCharacter.id, pos);
            }
            else
            {
                CheckPositionValues(pos);
                _slots[characterPositions[_selectedCharacter]].CleanTransform();
                characterPositions[_selectedCharacter] = pos;
                ServiceRegistry.Characters.SetCharacterPosition(_selectedCharacter.id, pos);
            }

            var s = Instantiate(prefabSlot);
            s.SetData(_selectedCharacter);
            s.GetComponent<Image>().raycastTarget = false;
            _selectedCharacter = null;
            return s;
        }
        return null;
    }

    void CheckPositionValues(int pos)
    {
        if (characterPositions.ContainsValue(pos))
        {
            foreach (var item in characterPositions)
            {
                if (item.Value == pos)
                {
                    ServiceRegistry.Characters.SetCharacterPosition(item.Key.id, 10);
                    break;
                }
            }
        }
    }

    public void Close()
    {
        ViewUtility.ShowThenHide<StartView, BattleOrderView>();
    }

    public void OnModelChanged(CharactersModel model)
    {
        _heroesModel = model;
        Init();
    }
}
