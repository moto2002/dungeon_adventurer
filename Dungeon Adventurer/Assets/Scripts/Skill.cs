using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GeneralSkillUpgrade : AEffectUpgrade
{
    public int cooldown;
    public int manaCost;
    public string description;
    public TargetType targetType;
    public TargetSite targetSite;
    public TargetAmount targetCount;
    public bool[] possiblePositions = new bool[9];
    public bool[] possibleTargets = new bool[9];
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Custom/Skill")]
public class Skill : ScriptableObject
{
    public int id;
    public string displayName;
    public int cooldown;
    public int manaCost;
    public bool isPassive;
    public Sprite icon;
    public string description;
    public TargetType targetType;
    public TargetSite targetSite;
    public TargetAmount targetCount;
    public GeneralSkillUpgrade[] upgrades = new GeneralSkillUpgrade[10];

    [HideInInspector]
    public bool[] possiblePositions = new bool[9];
    [HideInInspector]
    public bool[] possibleTargets = new bool[9];

    public List<SkillEffect> SkillEffects = new List<SkillEffect>();
    [HideInInspector]
    public int RemainingCooldown;
    [HideInInspector]
    public bool HasCooldown => RemainingCooldown > 0;
    [HideInInspector]
    public bool CanBeCasted(Character character) => character.CurrentMana >= manaCost;

    [HideInInspector]
    public bool IsUsable(Character character)
    {
        return !HasCooldown && CanBeCasted(character);
    }

    public void CopyFrom(Skill skill)
    {
        id = skill.id;
        displayName = skill.displayName;
        cooldown = skill.cooldown;
        manaCost = skill.manaCost;
        isPassive = skill.isPassive;
        icon = skill.icon;
        description = skill.description;
        targetType = skill.targetType;
        targetCount = skill.targetCount;
        targetSite = skill.targetSite;
        possiblePositions = skill.possiblePositions;
        possibleTargets = skill.possibleTargets;
        SkillEffects = skill.SkillEffects;
        RemainingCooldown = skill.RemainingCooldown;
    }

    void AddNew()
    {
        SkillEffects.Add(new SkillEffect());
    }

    void Remove(int index)
    {
        SkillEffects.RemoveAt(index);
    }

    public void SetCooldown(int coolDown)
    {
        RemainingCooldown = coolDown;
    }

    public void ReduceCooldown(int value)
    {
        RemainingCooldown = Mathf.Max(RemainingCooldown - value, 0);
    }

    public void UseSkill(Character caster, Character[] targets, BattleView view)
    {
        foreach (var target in targets)
        {
            foreach (var skill in SkillEffects)
            {
                skill.effect.UseEffect(caster, target, view);
            }
        }
        RemainingCooldown = cooldown;
        caster.CurrentMana -= manaCost;
    }

    public void UseSkill(Character caster, BattleView view)
    {
        foreach (var skill in SkillEffects)
        {
            skill.effect.UseEffect(caster, view);
        }
        RemainingCooldown = cooldown;
    }

    public string GetDescription(Character caster)
    {
        var str = description;
        foreach (var effect in SkillEffects)
        {
            str = effect.effect.ReplaceString(caster, str);
        }

        return str;
    }

    public bool CheckForPossibleTarget(int pos) { return possibleTargets[pos]; }
    public bool CheckForPossible(int pos) { return possiblePositions[pos]; }

    public void OnValidate()
    {
        if (upgrades.Length != 10) upgrades = new GeneralSkillUpgrade[10];
    }
}

[System.Serializable]
public class SkillEffect
{
    public Effect effect;
}

public enum TargetType
{
    Character,
    Position,
    CharacterAndPosition
}

public enum TargetSite
{
    Self,
    Team,
    Enemy
}

public enum TargetAmount
{
    Single,
    Multiple,
    All,
    GlobalAll
}

public enum Receiver
{
    Target,
    Caster
}

