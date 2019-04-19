using UnityEngine;

[CreateAssetMenu]
public class DoT : Effect
{
    public int damageValue;
    public int turnCounts;
    public DoTDamageType type;

    public override void UseEffect(Character ch)
    {
        Debug.Log($"Deal {damageValue} {type} damage for {turnCounts} turns!!");
    }
}

public enum DoTDamageType
{ 
    Bleed,
    Burn,
    Freeze,
    Shock,
    Poision
}
