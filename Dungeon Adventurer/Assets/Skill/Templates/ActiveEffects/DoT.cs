#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DoTUpgrade : AEffectUpgrade
{
    public int damageFlat;
    public float damagePercentage;
    public int turnCounts;
    public DoTDamageType type;

    public DoTUpgrade Clone()
    {
        var rv = new DoTUpgrade();
        rv.damageFlat = damageFlat;
        rv.damagePercentage = damagePercentage;
        rv.turnCounts = turnCounts;
        rv.type = type;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New DoT", menuName = "Skill/Effect/DoT")]
public class DoT : Effect
{
    const string DotDmgFlatString = "_dotFlat_";
    const string DotDmgPercentageString = "_dotPer_";
    const string DotTurnString = "_dotTurn_";

    public int damageFlat;
    public float damagePercentage;
    public int turnCounts;
    public DoTDamageType type;
    public DoTUpgrade[] upgrades = new DoTUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        target.AddDot(new DotInfo(turnCounts, type, damageFlat, damagePercentage));
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(DotDmgFlatString))
            s = s.Replace(DotDmgFlatString, $"<color=#800000ff> {damageFlat} {type} Damage</color>");

        if (s.Contains(DotDmgPercentageString))
            s = s.Replace(DotDmgPercentageString, $"<color=#800000ff> {damagePercentage * 100} % {type} Damage</color>");

        if (s.Contains(DotTurnString))
        {
            var append = turnCounts <= 1 ? "Turn" : "Turns";
            s = s.Replace(DotTurnString, $"<color=#800000ff>{turnCounts} </color> " + append);
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
        if (upgrades.Length != 10) upgrades = new DoTUpgrade[10];
    }
}

public enum DoTDamageType
{
    Bleed,
    Burn,
    Freeze,
    Shock,
    Poison
}

public class DotInfo
{
    readonly Dictionary<DoTDamageType, DamageType> _mappedDamageTypes = new Dictionary<DoTDamageType, DamageType>()
    {
        {DoTDamageType.Bleed, DamageType.Physical},
        {DoTDamageType.Burn, DamageType.Fire},
        {DoTDamageType.Freeze, DamageType.Cold},
        {DoTDamageType.Shock, DamageType.Lightning},
        {DoTDamageType.Poison, DamageType.Chaos}
    };

    public int TurnCount;
    public DamageType DamageType;
    public DoTDamageType Type;
    public int FlatDamage;
    public float PercentageDamage;

    public DotInfo(int turnCount, DoTDamageType type, int flatDamage, float percentageDamage)
    {
        TurnCount = turnCount;
        Type = type;
        DamageType = _mappedDamageTypes[Type];
        FlatDamage = flatDamage;
        PercentageDamage = percentageDamage;
    }

    public void AddDotInfo(DotInfo addedInfo)
    {
        TurnCount += addedInfo.TurnCount;
        FlatDamage += addedInfo.FlatDamage;
        PercentageDamage += addedInfo.PercentageDamage;
    }
}
