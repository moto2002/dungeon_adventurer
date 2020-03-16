#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class HealUpgrade : AEffectUpgrade
{
    public int valueFlat;
    public int valuePercentage;
    public Receiver receiver;

    public HealUpgrade Clone()
    {
        var rv = new HealUpgrade();
        rv.valueFlat = valueFlat;
        rv.valuePercentage = valuePercentage;
        rv.receiver = receiver;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New Heal", menuName = "Skill/Effect/Heal")]
public class Heal : Effect
{
    const string HealFlatDmgString = "_healFlat_";
    const string HealPerDmgString = "_healPer_";

    public int valueFlat;
    public int valuePercentage;
    public Receiver receiver;
    public HealUpgrade[] upgrades = new HealUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        switch(receiver){

            case Receiver.Target:
                target.ApplyHeal(target.MaxLife * valuePercentage + valueFlat);
                break;
            case Receiver.Caster:
                caster.ApplyHeal(caster.MaxLife * valuePercentage + valueFlat);
                break;
        }
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(HealFlatDmgString))
            s = s.Replace(HealFlatDmgString,
            $"heal {valueFlat} Hp");

        if (s.Contains(HealPerDmgString))
            s = s.Replace(HealPerDmgString,
            $"heal {valuePercentage * 100}% Hp");
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
        if (upgrades.Length != 10) upgrades = new HealUpgrade[10];
    }
}



