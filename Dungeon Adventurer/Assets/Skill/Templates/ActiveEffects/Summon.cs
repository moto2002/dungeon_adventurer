#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using static Summon;

[Serializable]
public class SummonUpgrade : AEffectUpgrade
{
    public Monster monster;
    public SummonPositions position;
    public int turnCounts;

    public SummonUpgrade Clone()
    {
        var rv = new SummonUpgrade();
        rv.monster = monster;
        rv.position = position;
        rv.turnCounts = turnCounts;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New Summon", menuName = "Skill/Effect/Summon")]
public class Summon : Effect
{
    const string SummonString = "_summon_";

    public Monster monster;
    public SummonPositions position;
    public int turnCounts;
    public SummonUpgrade[] upgrades = new SummonUpgrade[10];

    public override void UseEffect(Character caster, BattleView view)
    {
        view.Summon(caster, monster, position, turnCounts);
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(SummonString))
        {
            var append = turnCounts <= 1 ? "Turn" : "Turns";
            s = s.Replace(SummonString, $" Summon a {monster.name} at {position.ToString()} Position for {turnCounts} " + append);
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

    [Serializable]
    public enum SummonPositions
    {
        Target,
        Close,
        Random,
        Front,
        Middle,
        Back
    }
}

