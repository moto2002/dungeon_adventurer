using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillEntry : MonoBehaviour, IPointerClickHandler {
    [SerializeField] Image backgroundImage;
    [SerializeField] Image skillImage;
    [SerializeField] Sprite placeHolder;

    Action<int> onClick;
    int id = 0;
    bool _showingPlaceHolder = false;

    public void OnPointerClick(PointerEventData eventData) {
        if (_showingPlaceHolder) return;
        backgroundImage.color = Colors.BATTLE_ACTIVE;
        onClick(id);
    }

    public void SetColor(Color c) {
        backgroundImage.color = c;
    }

    public void SetData(Skill skill, int id, Action<int> onClick) {
        this.id = id;
        skillImage.sprite = skill.icon;
        this.onClick = onClick;
        _showingPlaceHolder = false;
    }

    public void EnablePlaceHolder() {
        _showingPlaceHolder = true;
        skillImage.sprite = placeHolder;
    }
}
