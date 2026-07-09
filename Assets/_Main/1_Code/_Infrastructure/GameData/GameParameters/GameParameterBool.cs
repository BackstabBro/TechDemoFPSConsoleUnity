using System;

public class GameParameterBool : IGameParameter<bool>
{
    public string Name { get; private set; }
    public bool Value => _value;
    public bool DefaultValue { get; }

    public event Action<IGameParameter> OnChanged;

    private bool _value;

    public GameParameterBool(string name, bool initialValue)
    {
        Name = name;
        DefaultValue = initialValue;
        Set(initialValue);
    }

    public void Set(bool value)
    {
        if (_value == value) return;
        _value = value;
        OnChanged?.Invoke(this);
    }

    public void Set(string textValue)
    {
        string cleanValue = textValue.Trim().ToLower();

        if (cleanValue == "1") { Set(true); return; }
        if (cleanValue == "0") { Set(false); return; }

        if (bool.TryParse(cleanValue, out bool result))
        {
            Set(result);
        }
    }

    public bool IsValueValid(string textValue)
    {
        if (string.IsNullOrWhiteSpace(textValue)) return false;

        string cleanValue = textValue.Trim().ToLower();

        if (cleanValue == "1" || cleanValue == "0") return true;

        return bool.TryParse(cleanValue, out _);
    }

    public string AsString() => _value ? "1" : "0";


    public void ResetToDefault()
    {
        Set(DefaultValue);
    }
}
