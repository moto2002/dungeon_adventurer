using UnityEngine;

[CreateAssetMenu]
public class ManaCost : Cost {
	public int manaDrain;

	public override bool TryApplyCost ( Stats stats ) {
		if( stats.mana < manaDrain ) return false;
		stats.mana -= manaDrain;
		return true;
	}

	public override bool ValidateCost ( Stats stats ) {
		return stats.mana > manaDrain;
	}
}
