#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using static IncreaseTurnCount;

[Serializable]
public class IncreaseTurnCountUpgrade : AEffectUpgrade
{
    public int value;
    public AffectedTypes affectedType;

    public IncreaseTurnCountUpgrade Clone()
    {
        var rv = new IncreaseTurnCountUpgrade();
        rv.value = value;
        rv.affectedType = affectedType;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New IncreaseTurnCount", menuName = "Skill/Effect/IncreaseTurnCount")]
public class IncreaseTurnCount : Effect
{
    string ValueString => $"_{affectedType}_change_";

    public int value;
    public AffectedTypes affectedType;
    public IncreaseTurnCountUpgrade[] upgrades = new IncreaseTurnCountUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        target.ChangeTurnCount(affectedType, value);
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(ValueString))
        {
            var prepend = value < 0? "decrease " : "increase ";
            s = s.Replace(ValueString, prepend + $"{affectedType} by {value}");
        }

        return s;
    }

#if UNITY_EDITOR
    public override void ApplyUpgrade(int level)
    {
        for (var i = level; i < upgrades.Length; i++)
        {
            upgrades[i] = upgrades[level].Clone();
        }
        EditorUtility.SetDirty(this);
    }
#endif

    public void OnValidate()
    {
        if (upgrades.Length != 10) upgrades = new IncreaseTurnCountUpgrade[10];
    }

    [Serializable]
    public enum AffectedTypes
    {
        Buffs,
        Dots,
        Markers,
        Status
    }
}


