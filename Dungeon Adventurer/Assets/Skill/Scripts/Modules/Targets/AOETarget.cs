using UnityEngine;

[CreateAssetMenu]
public class AOETarget : Target {
	public enum TargetOrigin {
		Unit,
		Mouse,
	}

	public TargetOrigin location;

	public override TargetGroup GetTargets () {
		throw new System.NotImplementedException();
	}
}
