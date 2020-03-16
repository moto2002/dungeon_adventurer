#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class ApplyStatusUpgrade : AEffectUpgrade
{
    public int turnCount;
    public float applyPercentage;
    public StatusType type;

    public ApplyStatusUpgrade Clone()
    {
        var rv = new ApplyStatusUpgrade();
        rv.turnCount = turnCount;
        rv.applyPercentage = applyPercentage;
        rv.type = type;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New ApplyStatus", menuName = "Skill/Effect/ApplyStatus")]
public class ApplyStatus : Effect
{
    string StatusString => $"_{type.ToString()}_value_";

    public int turnCount;
    public float applyPercentage;
    public StatusType type;
    public ApplyStatusUpgrade[] upgrades = new ApplyStatusUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        target.AddStatus(new StatusInfo(turnCount, type));
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(StatusString))
        {
            var append = turnCount <= 1 ? " Turn" : " Turns";
            s = s.Replace(StatusString, $"{applyPercentage * 100}% Chance to apply {type.ToString()} for {turnCount}" + append);
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
        if (upgrades.Length != 10) upgrades = new ApplyStatusUpgrade[10];
    }
}

[Serializable]
public enum StatusType
{
    Freeze,
    Stun,
    Shock,
    Burn,
    Pinned,
    KnockedDown,
    Paralysed,
    Petrified,
    Blind
}

public class StatusInfo
{

    public StatusType Type;
    public int TurnCount;

    public StatusInfo(int turnCount, StatusType type)
    {
        TurnCount = turnCount;
        Type = type;
    }

}
