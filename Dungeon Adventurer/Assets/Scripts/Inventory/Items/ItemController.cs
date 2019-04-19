using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IPointerClickHandler
{
    private Item _appliedItem;

    private Action onButtonClick;

    [SerializeField] Image rarityImage;
    [SerializeField] Image typeImage;


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
