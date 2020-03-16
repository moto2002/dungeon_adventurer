#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class MagicDamageUpgrade : AEffectUpgrade
{
    public int damageMin;
    public int damageMax;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;

    public MagicDamageUpgrade Clone()
    {
        var rv = new MagicDamageUpgrade();
        rv.damageMin = damageMin;
        rv.damageMax = damageMax;
        rv.type = type;
        rv.lifeSteal = lifeSteal;
        rv.ignoreResistance = ignoreResistance;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New MagicDamage", menuName = "Skill/Effect/MagicDamage")]
public class MagicDamage : Effect
{
    const string DmgString = "_m_DmgValue_";
    const string LifeStealString = "_m_Lifesteal_";
    const string IgnoreResistanceString = "_m_IgnoreRes_";

    public int damageMin;
    public int damageMax;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;
    public MagicDamageUpgrade[] upgrades = new MagicDamageUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        float amount = UnityEngine.Random.Range(damageMin, damageMax + 1);
        amount = amount + amount * (caster.Sub.MagicDamage / 100f);

        var appliedDamage = target.ApplyDamage(caster, type, amount, ignoreResistance);

        if (lifeSteal != 0)
        {
            caster.ApplyHeal((int)(appliedDamage * lifeSteal));
        }
    }

    public override string ReplaceString(Character caster, string s)
    {

        if (caster.GetType() == typeof(Hero))
        {
            var hero = caster as Hero;

            var color = Colors.HexByDamageType(type);

            if (s.Contains(DmgString))
            {
                s = s.Replace(DmgString,
                $"<color={color}>{(int)(damageMin * hero.Sub.MagicDamage)} - {(int)(damageMax * hero.Sub.MagicDamage)} {type.ToString()} Damage</color>");
            }

            if (s.Contains(LifeStealString))
            {
                s = s.Replace(LifeStealString, $"{lifeSteal * 100}%");
            }

            if (s.Contains(IgnoreResistanceString))
            {
                s = s.Replace(IgnoreResistanceString, $"{ignoreResistance * 100}%");
            }
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
        if (upgrades.Length != 10) upgrades = new MagicDamageUpgrade[10];
    }
}
