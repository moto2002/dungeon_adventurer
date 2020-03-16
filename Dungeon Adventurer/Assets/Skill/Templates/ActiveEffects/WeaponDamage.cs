#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public abstract class AEffectUpgrade
{
    [HideInInspector]
    public int level;
}

[Serializable]
public class WeaponDamageUpgrade : AEffectUpgrade
{
    public float damagePercentage;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;

    public WeaponDamageUpgrade Clone()
    {
        var rv = new WeaponDamageUpgrade();
        rv.damagePercentage = damagePercentage;
        rv.type = type;
        rv.lifeSteal = lifeSteal;
        rv.ignoreResistance = ignoreResistance;
        return rv;
    }
}

[Serializable]
[CreateAssetMenu(fileName = "New WeaponDamage", menuName = "Skill/Effect/WeaponDamage")]
public class WeaponDamage : Effect
{
    const string DmgPercentageString = "_p_DmgPercentage_";
    const string LifeStealString = "_p_Lifesteal_";
    const string IgnoreResistanceString = "_p_IgnoreRes_";

    public float damagePercentage;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;
    public WeaponDamageUpgrade[] upgrades = new WeaponDamageUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        var amount = 0f;
        if (caster.GetType() == typeof(Hero))
        {
            var hero = caster as Hero;
            amount = UnityEngine.Random.Range(hero.MinPhysicalDamage, hero.MaxPhysicalDamage + 1);
        } else
        {
            var monster = target as Monster;
            amount = UnityEngine.Random.Range(monster.minPhysicalDamage, monster.maxPhysicalDamage + 1);
        }

        amount = amount * damagePercentage;
        var appliedDamage = target.ApplyDamage(caster, type, amount, ignoreResistance);

        if (lifeSteal != 0)
        {
            caster.ApplyHeal(appliedDamage);
        }
    }

    public override string ReplaceString(Character caster, string s)
    {

        if (caster.GetType() == typeof(Hero))
        {
            var hero = caster as Hero;

            var color = Colors.HexByDamageType(type);

            if (s.Contains(DmgPercentageString))
            {
                s = s.Replace(DmgPercentageString,
                $"<color={color}>{(int)(hero.MinPhysicalDamage * damagePercentage)} - {(int)(hero.MaxPhysicalDamage * damagePercentage)} {type.ToString()} Damage</color>");
            }

            if (s.Contains(LifeStealString))
            {
                s = s.Replace(LifeStealString, $"{lifeSteal * 100} %");
            }

            if (s.Contains(IgnoreResistanceString))
            {
                s = s.Replace(IgnoreResistanceString, $"{ignoreResistance * 100} %");
            }
        }
        return s;
    }



    public override void ApplyLevel(int level)
    {
        base.ApplyLevel(level);
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

    public void OnValidate()
    {
        var newUpgrade = new WeaponDamageUpgrade()
        {
            damagePercentage = damagePercentage,
            lifeSteal = lifeSteal,
            type = type,
            ignoreResistance = ignoreResistance
        };

        for (var i = 0; i < upgrades.Length; i++)
        {
            upgrades[i] = newUpgrade;
        }
        EditorUtility.SetDirty(this);
    }
#endif

}

[Serializable]
public enum DamageType
{
    Physical,
    Fire,
    Lightning,
    Cold,
    Chaos
}
