#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using static DamageInfluence;

[Serializable]
public class DamageInfluenceUpgrade : AEffectUpgrade
{
    public float damageChangePercentage;
    public int turnCounts;
    public InfluenceSource source;

    public DamageInfluenceUpgrade Clone()
    {
        var rv = new DamageInfluenceUpgrade();
        rv.damageChangePercentage = damageChangePercentage;
        rv.turnCounts = turnCounts;
        rv.source = source;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New DamageInfluence", menuName = "Skill/Effect/DamageInfluence")]
public class DamageInfluence : Effect
{
    public float damageChangePercentage;
    public int turnCounts;
    public InfluenceSource source;
    public DamageInfluenceUpgrade[] upgrades = new DamageInfluenceUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        target.AddMarker(new DamageMarkerInfo(caster, damageChangePercentage, source, turnCounts), false);
    }

    public override string ReplaceString(Character caster, string s)
    {

        if (s.Contains("_dotTurn_"))
        {
            var append = turnCounts <= 1 ? "Turn" : "Turns";
            s = s.Replace("_dotTurn_", $"<color=#800000ff>{turnCounts}</color> " + append);
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
        if (upgrades.Length != 10) upgrades = new DamageInfluenceUpgrade[10];
    }

    [Serializable]
    public enum InfluenceSource
    {
        Caster,
        All,
        AllButCaster
    }
}

