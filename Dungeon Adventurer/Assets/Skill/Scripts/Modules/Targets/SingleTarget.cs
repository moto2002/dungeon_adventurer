using UnityEngine;

[CreateAssetMenu]
public class SingleTarget : Target {
	public bool targetSelf;

	public override TargetGroup GetTargets () {
		throw new System.NotImplementedException();
	}
}
