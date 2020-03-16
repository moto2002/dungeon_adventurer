using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect : ScriptableObject
{
    public virtual void UseEffect(Character caster, Character target, BattleView view)
    {
        Debug.LogError("Using UseEffect(Character, Character) without override");
    }

    public virtual void UseEffect(Character caster, Character target, Character additionalTarget, BattleView view)
    {
        Debug.LogError("Using UseEffect(Character, Character, Character) without override");
    }

    public virtual void UseEffect(Character caster, BattleView view)
    {
        Debug.LogError("Using UseEffect(Character) without override");
    }

    public virtual string ReplaceString(Character caster, string s) { return ""; }

    public virtual void ApplyLevel(int level) { }

#if UNITY_EDITOR
    public virtual void ApplyUpgrade(int level) { }
#endif

}

