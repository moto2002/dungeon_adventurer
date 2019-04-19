using UnityEngine;

[CreateAssetMenu]
public class EnergyCost : Cost {
	public float energyDrain;

	public override bool TryApplyCost ( Stats stats ) {
		if( stats.energy < energyDrain ) return false;
		stats.energy -= energyDrain;
		return true;
	}

	public override bool ValidateCost ( Stats stats ) {
		return stats.energy > energyDrain;
	}
}
