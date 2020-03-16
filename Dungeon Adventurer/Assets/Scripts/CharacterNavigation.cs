public class CharacterNavigation : ANavigation<ACharacterNavigationEntry>
{
}

public class ACharacterNavigationEntry : ANavigationEntry
{
    public virtual void RefreshHero(Hero hero) { }
}
