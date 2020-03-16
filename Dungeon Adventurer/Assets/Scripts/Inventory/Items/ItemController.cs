using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image rarityImage;
    [SerializeField] Image typeImage;

    public Item AppliedItem => _appliedItem;

    Item _appliedItem;
    Action onButtonClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        onButtonClick();
    }

    public void SetData(Item item, Action action) {

        _appliedItem = item;
        onButtonClick = action;
        Init();
    }

    void Init() {

        rarityImage.color = Colors.ByRarity(_appliedItem.rarity);
        typeImage.sprite = _appliedItem.icon;
    }
}
