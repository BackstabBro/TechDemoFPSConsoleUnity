using System;
using System.Globalization;
using Unity.VisualScripting;

public class GameParameterTimeSpan : IGameParameter<TimeSpan>
{
    public string Name { get; private set; }
    public TimeSpan Value => _value;
    public TimeSpan MinValue { get; }
    public TimeSpan MaxValue { get; }
    public TimeSpan DefaultValue { get; }
    public event Action<IGameParameter> OnChanged;

    private TimeSpan _value;

    public GameParameterTimeSpan(string name, TimeSpan initialValue, TimeSpan? min = null, TimeSpan? max = null)
    {
        Name = name;
        MinValue = min ?? TimeSpan.MinValue;
        MaxValue = max ?? TimeSpan.MaxValue;
        DefaultValue = initialValue;
        Set(initialValue);
    }

    public void Set(TimeSpan value)
    {
        if (value < MinValue) value = MinValue;
        if (value > MaxValue) value = MaxValue;

        if (_value == value) return;

        _value = value;
        OnChanged?.Invoke(this);
    }

    public void Set(string textValue)
    {
        if (TimeSpan.TryParse(textValue, CultureInfo.InvariantCulture, out TimeSpan result))
        {
            Set(result);
        }
    }

    public string AsString() => _value.ToString();

    public bool IsValueValid(string textValue) =>
        TimeSpan.TryParse(textValue, CultureInfo.InvariantCulture, out _);

    public void ResetToDefault()
    {
        Set(DefaultValue);
    }
}
