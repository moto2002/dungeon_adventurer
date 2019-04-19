using System;
using UnityEngine;

[CreateAssetMenu]
public class Heal : Effect
{
    public int value;

    public override void UseEffect(Character ch)
    {
        Debug.Log($"Restore {value} life");
    }
}

