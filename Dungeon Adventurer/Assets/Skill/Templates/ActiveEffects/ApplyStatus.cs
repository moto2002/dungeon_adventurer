using System;
using UnityEngine;

[CreateAssetMenu]
public class ApplyStatus : Effect
{
    public int turnCount;
    public StatusType type;

    public override void UseEffect(Character ch)
    {
        ch.AddStatus(type, turnCount);
        Debug.Log($"Apply {type} for {turnCount}");
    }
}

[Serializable]
public enum StatusType
{
    Freeze,
    Stun,
    Shock,
    Burn,
    Pinned,
    KnockedDown,
    Paralysed,
    Petrified
}
