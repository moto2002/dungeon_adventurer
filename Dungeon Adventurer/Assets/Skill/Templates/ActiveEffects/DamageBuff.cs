#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class DamageBuffUpgrade : AEffectUpgrade
{
    public int damageChangeFlat;
    public float damageChangePercentage;
    public int damageIncPerLifeLost;
    public Receiver receiver;
    public int turnCounts;

    public DamageBuffUpgrade Clone()
    {
        var rv = new DamageBuffUpgrade();
        rv.damageChangeFlat = damageChangeFlat;
        rv.damageChangePercentage = damageChangePercentage;
        rv.damageIncPerLifeLost = damageIncPerLifeLost;
        rv.receiver = receiver;
        rv.turnCounts = turnCounts;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New DamageBuff", menuName = "Skill/Effect/DamageBuff")]
public class DamageBuff : Effect
{
    public int damageChangeFlat;
    public float damageChangePercentage;
    public int damageIncPerLifeLost;
    public Receiver receiver;
    public int turnCounts;
    public DamageBuffUpgrade[] upgrades = new DamageBuffUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        var character = receiver == Receiver.Caster ? caster : target;

        character.AddBuff(new BuffInfo(SubStat.PhysicalDamage, turnCounts, damageChangeFlat, damageChangePercentage, damageIncPerLifeLost));
        character.AddBuff(new BuffInfo(SubStat.PhysicalDamage, turnCounts, damageChangeFlat, damageChangePercentage, damageIncPerLifeLost));
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains("_dotDmg_"))
            s = s.Replace("_dotDmg_", $"<color=#800000ff>{damageChangeFlat}</color>");

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
        if (upgrades.Length != 10) upgrades = new DamageBuffUpgrade[10];
    }
}

