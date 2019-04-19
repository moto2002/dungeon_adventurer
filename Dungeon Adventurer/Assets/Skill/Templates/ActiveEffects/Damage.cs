using System;
using UnityEngine;

[CreateAssetMenu]
public class Damage : Effect
{
    public int value;
    public DamageType type;

    public override void UseEffect(Character ch)
    {
        var res = ch.sub.armor;
        switch (type) {
            case DamageType.Physical:
                res = ch.sub.armor;
                break;
            case DamageType.Fire:
                res = ch.sub.fireRes;
                break;
            case DamageType.Lightning:
                res = ch.sub.lightRes;
                break;
            case DamageType.Cold:
                res = ch.sub.iceRes;
                break;
            case DamageType.Chaos:
                res = 0;
                break;
        }
        var dmg = (int)(value * ((100f - res) / 100f));
        ch.CurrentLife -= dmg;
        Debug.Log($"Deal {dmg} {type} damage to {ch.name} !!");
    }
}

[Serializable]
public enum DamageType
{
    Physical,
    Fire,
    Lightning,
    Cold,
    Chaos
}
