#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[Serializable]
public class HealPerTurnUpgrade : AEffectUpgrade
{
    public int amountFlat;
    public float amountPercentage;
    public int turnCount;

    public HealPerTurnUpgrade Clone()
    {
        var rv = new HealPerTurnUpgrade();
        rv.amountFlat = amountFlat;
        rv.amountPercentage = amountPercentage;
        rv.turnCount = turnCount;
        return rv;
    }
}

[CreateAssetMenu (fileName = "New HealPerTurn", menuName = "Skill/Effect/HealPerTurn")]
public class HealPerTurn : Effect
{
    public int amountFlat;
    public float amountPercentage;
    public int turnCount;
    public HealPerTurnUpgrade[] upgrades = new HealPerTurnUpgrade[10];

    public override void UseEffect(Character caster, Character target, BattleView view){
        Debug.Log($"Heal {amountFlat} for {turnCount} turns");
    }

    public override string ReplaceString(Character caster, string s) {
        if (s.Contains("_hptAmount_"))
            s = s.Replace("_hptAmount_", $"<color=#800000ff>{amountFlat}</color>");

        if (s.Contains("_hptTurn_")) {
            var append = turnCount <= 1 ? "Turn" : "Turns";
            s = s.Replace("_hptTurn_", $"<color=#800000ff>{turnCount}</color> " + append);
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
        if (upgrades.Length != 10) upgrades = new HealPerTurnUpgrade[10];
    }
}
