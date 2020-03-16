public class ServiceRegistry
{
    public static CurrencyService Currency;
    public static CharactersService Characters;
    public static DungeonService Dungeon;
    public static InventoryService Inventory;
    public static TimeProvider Time;

    public static void Init()
    {
        Currency = new CurrencyService();
        Characters = new CharactersService();
        Dungeon = new DungeonService();
        Inventory = new InventoryService();
        Time = TimeProvider.Instance;
    }

    public static void AddListener(EventListener listener)
    {
        Currency.RegisterListener(listener);
        Characters.RegisterListener(listener);
        Dungeon.RegisterListener(listener);
        Inventory.RegisterListener(listener);
        Time.RegisterListener(listener);
    }

    public static void RemoveListener(EventListener listener)
    {
        Currency.UnregisterListener(listener);
        Characters.UnregisterListener(listener);
        Dungeon.UnregisterListener(listener);
        Inventory.UnregisterListener(listener);
        Time.UnregisterListener(listener);
    }
}
