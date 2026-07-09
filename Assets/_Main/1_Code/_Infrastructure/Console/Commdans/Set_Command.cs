using VContainer;


public class Set_Command : IConsoleCommand
{
    public string Name => "set";
    public string Description => "Changes game variables. Usage: set [param_name] [value]";

    private readonly GameData _gameData;

    [Inject]
    public Set_Command(GameData gameData)
    {
        _gameData = gameData;
    }



    public string Execute(string[] args)
    {
        if (args == null || args.Length < 2 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
            return "[CONSOLE ERROR] Example: set pm_speed 6.5 || set pm_noclip true";

        string paramName = args[0];
        string valueToSet = args[1];

        if (_gameData.TrySetParam(paramName, valueToSet, out string finalValue))
        {
            return $"[CONSOLE] {paramName.ToLower()} = \"{finalValue}\"";
        }

        return $"[CONSOLE ERROR] Param '{paramName}' not registred or incorrect value type";
    }
}
