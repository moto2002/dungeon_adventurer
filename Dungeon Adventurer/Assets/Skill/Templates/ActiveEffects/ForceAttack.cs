#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using static ForceAttack;

[Serializable]
public class ForceAttackUpgrade : AEffectUpgrade
{
    public float damagePercentage;
    public DamageType damageType;
    public AttackMode attackMode;

    public ForceAttackUpgrade Clone()
    {
        var rv = new ForceAttackUpgrade();
        rv.damagePercentage = damagePercentage;
        rv.damageType = damageType;
        rv.attackMode = attackMode;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New ForceAttack", menuName = "Skill/Effect/ForceAttack")]
public class ForceAttack : Effect
{
    public float damagePercentage;
    public DamageType damageType;
    public AttackMode attackMode;
    public ForceAttackUpgrade[] upgrades = new ForceAttackUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        if (attackMode == AttackMode.Target)
        {
            Debug.LogWarning("use other UseEffect(Character, Character, Character)");
            return;
        }

        view.ForceAttack(target, damagePercentage, damageType, attackMode);
    }

    public override void UseEffect(Character caster, Character target, Character additionalTarget, BattleView view)
    {
        if (attackMode != AttackMode.Target)
        {
            UseEffect(caster, target, view);
            return;
        }

        var amount = 0f;
        if (target.GetType() == typeof(Hero))
        {
            var hero = target as Hero;
            amount = UnityEngine.Random.Range(hero.MinPhysicalDamage, hero.MaxPhysicalDamage + 1);
        }
        else
        {
            var monster = target as Monster;
            amount = UnityEngine.Random.Range(monster.minPhysicalDamage, monster.maxPhysicalDamage + 1);
        }

        amount = amount * damagePercentage;
        var appliedDamage = additionalTarget.ApplyDamage(target, damageType, amount, 0);
    }

    public override string ReplaceString(Character caster, string s)
    {
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
        if (upgrades.Length != 10) upgrades = new ForceAttackUpgrade[10];
    }

    [Serializable]
    public enum AttackMode
    {
        Target,
        Random,
        RandomClose,
        AllClose
    }
}

