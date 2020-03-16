public class Progress
{
    int _minValue;
    int _maxValue;
    int _currentValue;

    public int MinValue => _minValue;
    public int MaxValue => _maxValue;
    public int Current => _currentValue;
    public bool Full => _currentValue == _maxValue;
    public bool Empty => _currentValue == _minValue;

    public Progress(int min, int max, int current)
    {
        _minValue = min;
        _maxValue = max;
        _currentValue = current;
    }

    public void RefreshValues(int newMin, int newMax, int newCurrent)
    {
        _minValue = newMin;
        _maxValue = newMax;
        _currentValue = newCurrent;
    }
}
