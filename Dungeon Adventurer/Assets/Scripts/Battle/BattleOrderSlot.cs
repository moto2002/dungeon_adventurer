using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleOrderSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image rarityBorder;
    [SerializeField] Image charImage;
    [SerializeField] GameObject selectionImage;

    Hero _appliedCharacter;

    public void SetData(Hero character)
    {
        _appliedCharacter = character;
        BattleOrderView.OnSlotSelected += Write;

        Init();
    }

    private void OnDisable()
    {
        BattleOrderView.OnSlotSelected -= Write;
    }

    void Init()
    {
        rarityBorder.color = Colors.ByRarity(_appliedCharacter.rarity);
        charImage.sprite = DataHolder._data.raceImages[(int)_appliedCharacter.race];
    }

    void Write()
    {
        selectionImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BattleOrderView._instance.CharacterSelected(_appliedCharacter);
        selectionImage.SetActive(true);
    }
}
