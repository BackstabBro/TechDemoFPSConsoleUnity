using System;
using System.Globalization;

public class GameParameterFloat : IGameParameter<float>
{
    public string Name { get; private set; }
    public float Value => _value;
    public float MinValue { get; }
    public float MaxValue { get; }
    public float DefaultValue { get; }
    public event Action<IGameParameter> OnChanged;

    private float _value;

    public GameParameterFloat(string name, float initialValue, float min = float.MinValue, float max = float.MaxValue)
    {
        Name = name;
        MinValue = min;
        MaxValue = max;
        DefaultValue = initialValue;
        Set(initialValue);
    }

    public void Set(float value)
    {
        value = Math.Clamp(value, MinValue, MaxValue);
        if (MathF.Abs(_value - value) < float.Epsilon) return;

        _value = value;
        OnChanged?.Invoke(this);
    }

    public void Set(string textValue)
    {
        if (float.TryParse(textValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            Set(result);
        }
    }

    public string AsString() => _value.ToString(CultureInfo.InvariantCulture);

    public bool IsValueValid(string textValue) =>
        float.TryParse(textValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _);


    public void ResetToDefault()
    {
        Set(DefaultValue); // ◄ Сбрасываем к исходному
    }
}
