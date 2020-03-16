#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class ConditionalMagicDamageUpgrade : AEffectUpgrade
{
    public int damageMin;
    public int damageMax;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;
    public ConditionType conditionType;
    public MarkerType conditionMarker;
    public StatusType conditionStatus;
    public float conditionValue;

    public ConditionalMagicDamageUpgrade Clone()
    {
        var rv = new ConditionalMagicDamageUpgrade();
        rv.damageMin = damageMin;
        rv.damageMax = damageMax;
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

[CreateAssetMenu(fileName = "New ConditionalMagicDamage", menuName = "Skill/Effect/ConditionalMagicDamage")]
public class ConditionalMagicDamage : Effect
{
    const string DmgMinString = "_cM_DamageMin_";
    const string DmgMaxString = "_cM_DamageMax_";
    const string DmgTypeString = "_cM_DamageType_";
    const string LifeStealString = "_cM_Lifesteal_";
    const string IgnoreResistanceString = "_cM_IgnoreRes_";
    const string ConditionTypeString = "_cM_Type_";
    const string ConditionMarkerString = "_cM_Marker_";
    const string ConditionStatusString = "_cM_Status_";
    const string ConditionValueString = "_cM_Value_";

    public int damageMin;
    public int damageMax;
    public DamageType type;
    public float lifeSteal;
    public float ignoreResistance;
    public ConditionType conditionType;
    public MarkerType conditionMarker;
    public StatusType conditionStatus;
    public float conditionValue;
    public ConditionalMagicDamageUpgrade[] upgrades = new ConditionalMagicDamageUpgrade[10];

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
                if(!caster.GetAppliedStatuses().ContainsKey(conditionStatus)) {return;}
                break;
        }

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
        if (upgrades.Length != 10) upgrades = new ConditionalMagicDamageUpgrade[10];
    }
}