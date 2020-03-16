#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using static DamageInfluence;

[Serializable]
public class ApplyMarkerUpgrade : AEffectUpgrade
{
    public Receivers markerReceiver;
    public int markerCount;
    public int turnCount;
    public MarkerType type;

    public ApplyMarkerUpgrade Clone()
    {
        var rv = new ApplyMarkerUpgrade();
        rv.markerReceiver = markerReceiver;
        rv.markerCount = markerCount;
        rv.turnCount = turnCount;
        rv.type = type;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New ApplyMarker", menuName = "Skill/Effect/ApplyMarker")]
public class ApplyMarker : Effect
{
    string MarkerString => $"_{type.ToString()}_value_";

    public Receivers markerReceiver;
    public int markerCount;
    public int turnCount;
    public MarkerType type;
    public ApplyMarkerUpgrade[] upgrades = new ApplyMarkerUpgrade[10];

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(MarkerString))
        {
            var append = turnCount <= 1 ? " Turn" : " Turns";
            s = s.Replace(MarkerString, $"apply {markerCount} {type.ToString()} for {turnCount}" + append);
        }

        return s;
    }

    public override void UseEffect(Character caster, Character ch, BattleView view)
    {
        var marker = new MarkerInfo(type, turnCount);
        ch.AddMarker(marker, true);
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
        if (upgrades.Length != 10) upgrades = new ApplyMarkerUpgrade[10];
    }
}

public class MarkerInfo
{
    public MarkerType Type;
    public int TurnCount;

    public MarkerInfo()
    {
    }

    public MarkerInfo(MarkerType type, int turnCount)
    {
        TurnCount = turnCount;
        Type = type;
    }
}

public class DamageMarkerInfo : MarkerInfo
{
    public Character Caster;
    public InfluenceSource InfluencedBy;
    public float DamageChange;

    public DamageMarkerInfo(Character caster, float damageChange, InfluenceSource influenced, int turnCount)
    {
        Caster = caster;
        DamageChange = damageChange;
        InfluencedBy = influenced;
        TurnCount = turnCount;
        Type = MarkerType.IncreasedDamage;
    }
}

[Serializable]
public enum MarkerType
{
    HuntersMark,
    AbyssalMark,
    AssasinationMark,
    SunsEmbrace,
    DamageProtection,
    GladiatorsTouch,
    IncreasedHealing,
    HolySign,
    IncreasedDamage
}

[Serializable]
public enum Receivers
{
    Self,
    Target,
    RandomAlly,
    RandomEnemy,
    All
}
