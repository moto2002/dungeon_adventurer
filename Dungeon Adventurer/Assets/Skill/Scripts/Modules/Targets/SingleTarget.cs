using UnityEngine;

[CreateAssetMenu (fileName = "New SingleTarget", menuName = "Skill/Effect/SingleTarget")]
public class SingleTarget : Target {
	public bool targetSelf;

	public override TargetGroup GetTargets () {
		throw new System.NotImplementedException();
	}
}
