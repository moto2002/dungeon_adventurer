public abstract class Cost : Module {
	public abstract bool ValidateCost ( Stats stats );
	public abstract bool TryApplyCost ( Stats stats );
}
