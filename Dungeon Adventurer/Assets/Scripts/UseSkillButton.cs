using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UseSkillButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI appliedText;
    [SerializeField] Button useButton;

    struct StatusInfo
    {
        public string buttontext;
        public bool interactable;
    }

    readonly StatusInfo NO_CHARACTER_SELECTED = new StatusInfo() { buttontext = "Select Character", interactable = false };
    readonly StatusInfo NO_POSITION_SELECTED = new StatusInfo() { buttontext = "Select Position", interactable = false };
    readonly StatusInfo NO_SKILL_SELECTED = new StatusInfo() { buttontext = "Select Skill", interactable = false };
    readonly StatusInfo NO_TARGET_AVAILABLE = new StatusInfo() { buttontext = "No Targets", interactable = false };
    readonly StatusInfo USE_SKILL = new StatusInfo() { buttontext = "Use Skill", interactable = true };
    readonly StatusInfo ENEMY_TURN = new StatusInfo() { buttontext = "Enemy Turn", interactable = false };

    public void SetData(Action callback)
    {
        useButton.onClick.AddListener(() => callback());
        BattleView.StatusChanged += ChangeStatus;
        ChangeStatus();
    }

    private void OnDisable()
    {
        BattleView.StatusChanged -= ChangeStatus;
    }

    void ChangeStatus()
    {
        var statusInfo = USE_SKILL;

        switch (BattleView.CurrentSkillStatus)
        {
            case SkillStatus.selectPosition:
                statusInfo = NO_POSITION_SELECTED;
                break;
            case SkillStatus.selectCharacter:
                statusInfo = NO_CHARACTER_SELECTED;
                break;
            case SkillStatus.noTargets:
                statusInfo = NO_TARGET_AVAILABLE;
                break;
            case SkillStatus.useSkill:
                statusInfo = USE_SKILL;
                break;
            case SkillStatus.selectSkill:
                statusInfo = NO_SKILL_SELECTED;
                break;
            case SkillStatus.enemyTurn:
                statusInfo = ENEMY_TURN;
                break;
        }
        appliedText.text = statusInfo.buttontext;
        useButton.interactable = statusInfo.interactable;
    }
}
