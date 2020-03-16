#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class HealAsDamageUpgrade : AEffectUpgrade
{
    public int healValueFlat;
    public float healValuePercentage;
    public float damageConversionPercentage;
    public DamageType type;

    public HealAsDamageUpgrade Clone()
    {
        var rv = new HealAsDamageUpgrade();
        rv.healValueFlat = healValueFlat;
        rv.healValuePercentage = healValuePercentage;
        rv.damageConversionPercentage = damageConversionPercentage;
        rv.type = type;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New HealAsDamage", menuName = "Skill/Effect/HealAsDamage")]
public class HealAsDamage : Effect
{
    const string HealFlatDmgString = "_healFlatDmg_";
    const string HealPerDmgString = "_healPerDmg_";

    public int healValueFlat;
    public float healValuePercentage;
    public float damageConversionPercentage;
    public DamageType type;
    public HealAsDamageUpgrade[] upgrades = new HealAsDamageUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        var healedAmount = healValueFlat != 0 ? target.ApplyHeal(healValueFlat) : target.ApplyHeal((int)(target.MaxLife * healValuePercentage));
        caster.ApplyDamage(caster, type, healedAmount * damageConversionPercentage, 0);
    }

    public override string ReplaceString(Character caster, string s)
    {
        var color = Colors.HexByDamageType(type);

        if (s.Contains(HealFlatDmgString))
            s = s.Replace(HealFlatDmgString,
            $"Heal {healValueFlat} Hp and suffer <color={color}>{damageConversionPercentage * healValueFlat} {type.ToString()} Damage</color>");

        if (s.Contains(HealPerDmgString))
            s = s.Replace(HealPerDmgString,
            $"Heal {healValuePercentage * 100}% Hp and suffer <color={color}>{damageConversionPercentage * 100}% {type.ToString()} Damage</color>");
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
        if (upgrades.Length != 10) upgrades = new HealAsDamageUpgrade[10];
    }
}

