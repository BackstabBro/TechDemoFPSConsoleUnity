using System;

public interface IGameParameter
{
    string Name { get; }
    string AsString();
    void Set(string textValue);
    bool IsValueValid(string textValue);

    event Action<IGameParameter> OnChanged;

    void ResetToDefault();

}

public interface IGameParameter<T> : IGameParameter
{
    T Value { get; }
    void Set(T value);
}
