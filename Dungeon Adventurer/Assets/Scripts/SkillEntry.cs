using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SkillEntry : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Image skillImage;
    [SerializeField] Sprite placeHolder;
    [SerializeField] GameObject cooldownContent;
    [SerializeField] GameObject noManaContent;
    [SerializeField] Image cooldownFill;
    [SerializeField] TextMeshProUGUI cooldownText;

    Action<int> onClick;
    int id = 0;
    bool _showingPlaceHolder = false;
    Skill _assignedSkill;
    Character _currentCharacter;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_showingPlaceHolder || !_assignedSkill.IsUsable(_currentCharacter))
        {
            return;
        }
        backgroundImage.color = Colors.BATTLE_ACTIVE;
        onClick(id);
    }

    public void SetColor(Color c)
    {
        backgroundImage.color = c;
    }

    public void SetData(Character currentCharacter, Skill skill, int id, Action<int> onClick)
    {
        _currentCharacter = currentCharacter;
        _assignedSkill = skill;
        this.id = id;
        skillImage.sprite = _assignedSkill.icon;
        this.onClick = onClick;
        _showingPlaceHolder = false;

        if (_assignedSkill.RemainingCooldown > 0)
        {
            cooldownText.text = _assignedSkill.RemainingCooldown + "";
            cooldownFill.fillAmount = (float)_assignedSkill.RemainingCooldown / _assignedSkill.cooldown;
            cooldownContent.SetActive(!_assignedSkill.HasCooldown);
        }
        else
        {
            noManaContent.SetActive(!_assignedSkill.CanBeCasted(_currentCharacter));
        }
    }

    public void EnablePlaceHolder()
    {
        _showingPlaceHolder = true;
        skillImage.sprite = placeHolder;
        cooldownContent.SetActive(false);
        noManaContent.SetActive(false);
    }
}
