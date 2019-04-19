using System;
using UnityEngine;

[CreateAssetMenu]
public class HealPerTurn : Effect
{
    public int amount;
    public int turnCount;

    public override void UseEffect(Character ch)
    {
        Debug.Log($"Heal {amount} for {turnCount} turns");
    }
}

