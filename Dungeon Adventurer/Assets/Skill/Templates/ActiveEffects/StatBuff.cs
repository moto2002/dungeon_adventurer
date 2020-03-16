#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class StatBuffUpgrade : AEffectUpgrade
{
    public int changeFlat;
    public float changePerc;
    public float changePerLifeLost;
    public SubStat stat;
    public Receiver receiver;
    public int turnCounts;

    public StatBuffUpgrade Clone()
    {
        var rv = new StatBuffUpgrade();
        rv.changeFlat = changeFlat;
        rv.changePerc = changePerc;
        rv.changePerLifeLost = changePerLifeLost;
        rv.stat = stat;
        rv.receiver = receiver;
        rv.turnCounts = turnCounts;
        return rv;
    }
}

[CreateAssetMenu(fileName = "New StatBuff", menuName = "Skill/Effect/StatBuff")]
public class StatBuff : Effect
{
    string FlatString => $"_{stat.ToString()}_flat_";
    string PercentageString => $"_{stat.ToString()}_per_";
    string ChangePerLifeString => $"_{stat.ToString()}_flatPerLife_";
    string ReceiverString => $"_{stat.ToString()}_receiver_";
    string CountString => $"_{stat.ToString()}_count_";

    public int changeFlat;
    public float changePerc;
    public float changePerLifeLost;
    public SubStat stat;
    public Receiver receiver;
    public int turnCounts;
    public StatBuffUpgrade[] upgrades = new StatBuffUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view)
    {
        var character = receiver == Receiver.Caster ? caster : target;
        character.AddBuff(new BuffInfo(stat, turnCounts, changeFlat, changePerc, changePerLifeLost));
    }

    public override string ReplaceString(Character caster, string s)
    {
        if (s.Contains(FlatString))
        {
            s = s.Replace(FlatString, $"<color=#800000ff>{stat} by {changeFlat}</color> ");
        }

        if (s.Contains(PercentageString))
        {
            s = s.Replace(PercentageString, $"<color=#800000ff>{stat} by {changePerc * 100}% </color> ");
        }

        if (s.Contains(ChangePerLifeString))
        {
            s = s.Replace(ChangePerLifeString, $"<color=#800000ff>{changePerLifeLost} {stat}</color> per 1% Life lost");
        }

        if (s.Contains(ReceiverString))
        {
            s = s.Replace(ReceiverString, $"<color=#800000ff>{receiver}</color> ");
        }

        if (s.Contains(CountString))
        {
            s = s.Replace(CountString, turnCounts > 1 ? $"<color=#800000ff>{turnCounts} Turns</color>" : $"<color=#800000ff>{turnCounts} Turn</color>");
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
        if (upgrades.Length != 10) upgrades = new StatBuffUpgrade[10];
    }
}

public class BuffInfo
{
    public SubStat AffectedStat;
    public int TurnCount;
    public int FlatChange;
    public float PercentageChange;
    public float ChangePerLifeLost;

    public BuffInfo(SubStat stat, int turnCount, int changeFlat, float changePerc, float changePerLifeLost)
    {
        AffectedStat = stat;
        TurnCount = turnCount;
        FlatChange = changeFlat;
        PercentageChange = changePerc;
        ChangePerLifeLost = changePerLifeLost;
    }

    public void AddBuffInfo(BuffInfo addedInfo)
    {
        TurnCount += addedInfo.TurnCount;
        FlatChange += addedInfo.FlatChange;
        PercentageChange += addedInfo.PercentageChange;
        ChangePerLifeLost += addedInfo.ChangePerLifeLost;
    }
}

