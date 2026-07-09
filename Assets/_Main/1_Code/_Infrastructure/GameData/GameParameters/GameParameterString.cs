using System;

public class GameParameterString : IGameParameter<string>
{
    public string Name { get; private set; }
    public string Value => _value;
    public string DefaultValue { get; }
    public event Action<IGameParameter> OnChanged;

    private string _value;

    public GameParameterString(string name, string initialValue)
    {
        Name = name;
        _value = initialValue;
        DefaultValue = initialValue;
    }

    public void Set(string value)
    {
        if (_value == value) return;

        _value = value;
        OnChanged?.Invoke(this);
    }

    void IGameParameter.Set(string textValue)
    {
        Set(textValue);
    }
    public string AsString() => _value;
    public bool IsValueValid(string textValue) => true;

    public void ResetToDefault()
    {
        Set(DefaultValue); // ◄ Сбрасываем к исходному
    }

}
