using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ConsoleCommandService : IDisposable
{
    public event Action<string> OnLogReceived;
    public event Action OnConsoleCleared;
    private readonly Dictionary<string, IConsoleCommand> _commands = new();
    private static readonly string[] EmptyArgs = Array.Empty<string>();
    private readonly GameData _gameData;

    [Inject]
    public ConsoleCommandService(IReadOnlyList<IConsoleCommand> commands, GameData gameData)
    {
        _gameData = gameData;


        foreach (var command in commands)
        {
            _commands[command.Name.ToLower()] = command;
        }
    }


    public void Init()
    {
        Application.logMessageReceived += HandleUnityLog;
    }


    public void RunCommand(string inputLine)
    {
        if (string.IsNullOrWhiteSpace(inputLine)) return;

        OnLogReceived?.Invoke($"> {inputLine}");
        inputLine = inputLine.Trim();

        int firstSpace = inputLine.IndexOf(' ');
        string commandName;
        string[] args;

        if (firstSpace == -1)
        {
            commandName = inputLine.ToLower();
            args = EmptyArgs;
        }
        else
        {
            commandName = inputLine.Substring(0, firstSpace).ToLower();
            string argsLine = inputLine.Substring(firstSpace + 1).Trim();
            args = string.IsNullOrWhiteSpace(argsLine)
                ? EmptyArgs
                : argsLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        // 1. Если это служебная команда (например: clear, help)
        if (_commands.TryGetValue(commandName, out var command))
        {
            ExecuteCommand(command, commandName, args);
        }
        // 2. Если это имя параметра из GameData
        else if (_gameData.HasParam(commandName))
        {
            // Если аргументов нет (просто "pm_speed") -> вызываем логику GET
            if (args.Length == 0)
            {
                if (_gameData.TryGetParam(commandName, out string finalValue, out string typeInfo))
                {
                    OnLogReceived?.Invoke($"[CONSOLE] {commandName} = \"{finalValue}\" ({typeInfo})");
                }
            }
            // Если есть аргумент (например "pm_speed 6.5") -> вызываем логику SET
            else
            {
                string valueToSet = args[0];
                if (_gameData.TrySetParam(commandName, valueToSet, out string finalValue))
                {
                    OnLogReceived?.Invoke($"[CONSOLE] {commandName} = \"{finalValue}\"");
                }
                else
                {
                    OnLogReceived?.Invoke($"[CONSOLE ERROR] Param '{commandName}' incorrect value type.");
                }
            }
        }
        else
        {
            OnLogReceived?.Invoke($"<color=yellow>[CONSOLE ERROR] Command or param '{commandName}' wasnt found.</color>");
        }
    }


    private void ExecuteCommand(IConsoleCommand command, string commandName, string[] args)
    {
        try
        {
            string response = command.Execute(args);
            if (!string.IsNullOrEmpty(response)) OnLogReceived?.Invoke(response);
        }
        catch (Exception ex)
        {
            Debug.LogError($"<color=red>Error executing command {commandName}: {ex.Message}</color>");
        }
    }


    public void AddCommand(IConsoleCommand consoleCommand) { _commands.Add(consoleCommand.Name, consoleCommand); }
    public void RemoveCommand(IConsoleCommand consoleCommand) { _commands.Remove(consoleCommand.Name); }

    public void ClearHistory()
    {
        OnConsoleCleared?.Invoke();
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        // Если лог пришел от самой консоли, не дублируем его, чтобы избежать бесконечного цикла
        if (logString.StartsWith("> ") || logString.StartsWith("[CONSOLE]")) return;

        string formattedLog = logString;

        // Меняем цвет через Rich Text
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                formattedLog = $"<color=red>[UNITY ERROR] {logString}</color>";
                break;
            case LogType.Warning:
                formattedLog = $"<color=yellow>[UNITY WARNING] {logString}</color>";
                break;
            case LogType.Log:
                formattedLog = $"[UNITY LOG] {logString}";
                break;
        }

        OnLogReceived?.Invoke(formattedLog);
    }

    public void Dispose()
    {
        Application.logMessageReceived -= HandleUnityLog;
    }
}
