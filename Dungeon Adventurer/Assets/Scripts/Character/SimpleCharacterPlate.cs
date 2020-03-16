using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimpleCharacterPlate : MonoBehaviour
{
    [SerializeField] Image characterIcon;
    [SerializeField] Image rarityBorder;
    [SerializeField] Button charButton;

    public void SetData(Hero hero, UnityAction callback)
    {
        charButton.onClick.RemoveAllListeners();
        charButton.onClick.AddListener(callback);

        characterIcon.sprite = hero.GetPortrait();
        rarityBorder.color = Colors.ByRarity(hero.rarity);
    }
}
