public class CurrencyService : EventPublisher
{
    CurrencyModel _currentCurrency;

    public interface OnCurrencyChanged
    {
        void OnModelChanged(CurrencyModel model);
    }
    private delegate void OnModelChanged(CurrencyModel model);
    private event OnModelChanged changedEvent;

    public override void RegisterListener(EventListener listener)
    {
        if (listener is OnCurrencyChanged)
        {
            var listen = (OnCurrencyChanged)listener;
            changedEvent += new OnModelChanged(listen.OnModelChanged);
            listen.OnModelChanged(_currentCurrency);
        }
    }

    public override void UnregisterListener(EventListener listener)
    {
        if (listener is OnCurrencyChanged)
            changedEvent -= new OnModelChanged(((OnCurrencyChanged)listener).OnModelChanged);
    }

    public CurrencyService() {
        Init();
    }

    public override void Init()
    {
        _currentCurrency = new CurrencyModel(DataHolder._data.GetCurrencyData());
        Publish();
    }

    public override void Publish()
    {
        if (changedEvent == null) return;
        changedEvent(_currentCurrency);
    }

    public void CreditCurrency(Currency cur, double amount) {
        _currentCurrency.CreditCurrency(cur, amount);
        Publish();
    }

    public bool TryPurchase(Currency cur, double value)
    {
        var rv = _currentCurrency.TryPurchase(cur, value);
        if(rv) Publish();
        return rv;
    }

    public CoinData ConvertDoubleToCoinData(double amount)
    {
        var rv = _currentCurrency.ConvertDoubleToCoinData(amount);
        return rv;
    }

}
