#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using static ChangePosition;

[Serializable]
public class ChangePositionUpgrade : AEffectUpgrade
{
    public MovedTarget target;
    public MoveDirection direction;

    public ChangePositionUpgrade Clone()
    {
        var rv = new ChangePositionUpgrade();
        rv.target = target;
        rv.direction = direction;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New ChangePosition", menuName = "Skill/Effect/ChangePosition")]
public class ChangePosition : Effect
{
    const string MoveTargetString = "_moveTarget_";
    const string MoveDirectionString = "_moveDirection_";

    public MovedTarget target;
    public MoveDirection direction;
    public ChangePositionUpgrade[] upgrades = new ChangePositionUpgrade[10];

    public override void UseEffect(Character caster, BattleView view)
    {
        //TODO Link to battleView
    }

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        //TODO Link to battleView
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(MoveTargetString))
            s = s.Replace(MoveTargetString, $"<color=#800000ff>{target}</color>");
        if (s.Contains(MoveDirectionString))
            s = s.Replace(MoveDirectionString, $"<color=#800000ff>{direction} Position</color>");

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
        if (upgrades.Length != 10) upgrades = new ChangePositionUpgrade[10];
    }

    [Serializable]
    public enum MovedTarget
    {
        Self,
        Target
    }

    [Serializable]
    public enum MoveDirection
    {
        Side,
        Front,
        Back,
        Random,
        Target
    }
}

