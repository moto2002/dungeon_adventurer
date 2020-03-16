using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon BaseItem", menuName = "Custom/WeaponBaseItem")]
public class Weapon : BaseItem
{
    public int MinDamage;
    public int MaxDamage;
    public int MinDamageIncrease;
    public int MaxDamageIncrease;
    public WeaponType WeaponType;
}

public enum WeaponType
{
    Axe,
    Hammer,
    Sword,
    Staff,
    Dagger
}
