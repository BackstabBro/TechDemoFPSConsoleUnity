using VContainer;

public class Get_Command : IConsoleCommand
{
    public string Name => "get";
    public string Description => "Displays the current value of a variable. Usage: get [param_name]";

    private readonly GameData _gameData;

    [Inject]
    public Get_Command(GameData gameData)
    {
        _gameData = gameData;
    }


    public string Execute(string[] args)
    {
        if (args == null || args.Length < 1 || string.IsNullOrEmpty(args[0]))
            return "[CONSOLE ERROR] Пример использования: get pm_speed";

        string paramName = args[0];

        if (_gameData.TryGetParam(paramName, out string finalValue, out string typeInfo))
        {
            return $"[CONSOLE] {paramName.ToLower()} = \"{finalValue}\" ({typeInfo})";
        }

        return $"[CONSOLE ERROR] Параметр '{paramName}' не зарегистрирован в системе.";
    }
}
