public class InventoryService : EventPublisher
{
    InventoryModel _currentInventory;

    public interface OnInventoryChanged
    {
        void OnModelChanged(InventoryModel model);
    }
    private delegate void OnModelChanged(InventoryModel model);
    private event OnModelChanged changedEvent;

    public override void RegisterListener(EventListener listener)
    {
        if (listener is OnInventoryChanged)
        {
            var listen = (OnInventoryChanged)listener;
            changedEvent += new OnModelChanged(listen.OnModelChanged);
            listen.OnModelChanged(_currentInventory);
        }
    }

    public override void UnregisterListener(EventListener listener)
    {
        if (listener is OnInventoryChanged)
            changedEvent -= new OnModelChanged(((OnInventoryChanged)listener).OnModelChanged);
    }

    public InventoryService()
    {
        Init();
    }

    public override void Init()
    {
        _currentInventory = new InventoryModel(DataHolder._data.LoadInventory());
        Publish();
    }

    public override void Publish()
    {
        DataHolder._data.SaveInventory(_currentInventory.InventoryItems);
        if (changedEvent == null) return;
        changedEvent(_currentInventory);
    }

    public void AddItemToInventory(ItemData data)
    {
        _currentInventory.AddItem(data);
        Publish();
    }

    public void RemoveItemFromInventory(ItemData data)
    {
        _currentInventory.RemoveItem(data);
        Publish();
    }

    public void SellItem(ItemData data, double value)
    {
        ServiceRegistry.Currency.CreditCurrency(Currency.Coins, value);
        _currentInventory.RemoveItem(data);
        Publish();
    }
}
