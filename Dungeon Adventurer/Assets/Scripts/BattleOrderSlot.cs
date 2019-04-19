using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleOrderSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    const float CLICK_WAIT_TIME = 0.4f;

    [SerializeField] Image rarityBorder;
    [SerializeField] Image charImage;
    [SerializeField] GameObject selectionImage;

    Hero _appliedCharacter;

    Coroutine coroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        coroutine = StartCoroutine(TrackClick());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }

    IEnumerator TrackClick()
    {
        yield return new WaitForSeconds(CLICK_WAIT_TIME);

        BattleOrderView._instance.CharacterSelected(_appliedCharacter);
        selectionImage.SetActive(true);
    }

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
}
