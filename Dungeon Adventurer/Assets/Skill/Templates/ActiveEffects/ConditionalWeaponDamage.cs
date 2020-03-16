#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class ConditionalWeaponDamageUpgrade : AEffectUpgrade
{
    public float damagePercentage;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;
    public ConditionType conditionType;
    public MarkerType conditionMarker;
    public StatusType conditionStatus;
    public float conditionValue;

    public ConditionalWeaponDamageUpgrade Clone()
    {
        var rv = new ConditionalWeaponDamageUpgrade();
        rv.damagePercentage = damagePercentage;
        rv.type = type;
        rv.lifeSteal = lifeSteal;
        rv.ignoreResistance = ignoreResistance;
        rv.conditionType = conditionType;
        rv.conditionMarker = conditionMarker;
        rv.conditionStatus = conditionStatus;
        rv.conditionValue = conditionValue;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New ConditionalWeaponDamage", menuName = "Skill/Effect/ConditionalWeaponDamage")]
public class ConditionalWeaponDamage : Effect
{
    const string DmgPercentageString = "_cP_DamagePercentage_";
    const string DmgTypeString = "_cP_DamageType_";
    const string LifeStealString = "_cP_Lifesteal_";
    const string IgnoreResistanceString = "_cP_IgnoreRes_";
    const string ConditionTypeString = "_cP_Type_";
    const string ConditionMarkerString = "_cP_Marker_";
    const string ConditionStatusString = "_cP_Status_";
    const string ConditionValueString = "_cP_Value_";

    public float damagePercentage;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;
    public ConditionType conditionType;
    public MarkerType conditionMarker;
    public StatusType conditionStatus;
    public float conditionValue;
    public ConditionalWeaponDamageUpgrade[] upgrades = new ConditionalWeaponDamageUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        switch (conditionType)
        {
            case ConditionType.Marker:
                if (!caster.GetAppliedMarkers().ContainsKey(conditionMarker)) { return; }
                break;
            case ConditionType.Life:
                if (!(caster.CurrentLife <= caster.MaxLife * conditionValue)) return;
                break;
            case ConditionType.Status:
                if (!caster.GetAppliedStatuses().ContainsKey(conditionStatus)) { return; }
                break;
        }

        var amount = 0f;
        if (caster.GetType() == typeof(Hero))
        {
            var hero = caster as Hero;
            amount = UnityEngine.Random.Range(hero.MinPhysicalDamage, hero.MaxPhysicalDamage + 1);
        }
        else
        {
            var monster = target as Monster;
            amount = UnityEngine.Random.Range(monster.minPhysicalDamage, monster.maxPhysicalDamage + 1);
        }

        amount = (int)(amount * damagePercentage);

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
                s = s.Replace(DmgPercentageString, $"<color={color}>{(int)(hero.MinPhysicalDamage * damagePercentage)} - {(int)(hero.MaxPhysicalDamage * damagePercentage)} {type} Damage</color>");
            }

            if (s.Contains(ConditionValueString))
            {
                s = s.Replace(ConditionValueString, $"if Life is lower than {conditionValue * 100} %");
            }

            if (s.Contains("_dmgValue_"))
            {
                s = s.Replace("_dmgValue_", $"<color={color}>{(int)(hero.MinPhysicalDamage * damagePercentage)} - {(int)(hero.MaxPhysicalDamage * damagePercentage)}</color>");
            }

            if (s.Contains("_dmgType_"))
            {
                s = s.Replace("_dmgType_", $"<color={color}>{type.ToString()}</color>");
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
        if (upgrades.Length != 10) upgrades = new ConditionalWeaponDamageUpgrade[10];
    }
}

[Serializable]
public enum ConditionType
{
    Marker,
    Life,
    Status
}
