using System;

public class GameParameterInt : IGameParameter<int>
{
    public string Name { get; private set; }
    public int Value => _value;
    public int MinValue { get; }
    public int MaxValue { get; }
    public int DefaultValue { get; }
    public event Action<IGameParameter> OnChanged;

    private int _value;

    public GameParameterInt(string name, int initialValue, int min = int.MinValue, int max = int.MaxValue)
    {
        Name = name;
        MinValue = min;
        MaxValue = max;
        DefaultValue = initialValue;
        Set(initialValue);
    }

    public void Set(int value)
    {
        value = Math.Clamp(value, MinValue, MaxValue);
        if (_value == value) return;

        _value = value;
        OnChanged?.Invoke(this);
    }

    public void Set(string textValue)
    {
        if (int.TryParse(textValue, out int result))
        {
            Set(result);
        }
    }

    public string AsString() => _value.ToString();
    public bool IsValueValid(string textValue) => int.TryParse(textValue, out _);

    public void ResetToDefault()
    {
        Set(DefaultValue);
    }
}
