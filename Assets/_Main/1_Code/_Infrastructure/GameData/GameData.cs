using System.Collections.Generic;
using VContainer;

public class GameData
{
    public PlayerParameters PlayerParameters { get; private set; }
    public TimeParameters TimeParameters { get; private set; }
    public GlobalParameters SettingsParameters { get; private set; }
    public GameTasksRepository GameTasksData { get; private set; }
    public Dictionary<string, IGameParameter> SessionParameters { get; private set; } //для сейвов
    public IReadOnlyList<ISaveableData> SessionClassesData { get; private set; } //для сейвов

    private readonly Dictionary<string, IGameParameter> _allParameters = new(); //для консоли



    [Inject]
    public GameData(
        PlayerParameters playerData, 
        TimeParameters timeData, 
        GameTasksRepository gameTasksData, 
        GlobalParameters globalData)
    {
        PlayerParameters = playerData;
        TimeParameters = timeData;
        SettingsParameters = globalData;
        GameTasksData = gameTasksData;
    }

    public void Init()
    {
        IReadOnlyList<GameParametersData> SessionGameParametersContainer;
        SessionGameParametersContainer = new List<GameParametersData> { PlayerParameters, TimeParameters };

        SessionParameters =new Dictionary<string, IGameParameter>();
        SessionClassesData = new List<ISaveableData> { GameTasksData };

        foreach (var container in SessionGameParametersContainer) AddGroupedParamsToDictionary(container, SessionParameters);
        foreach (var container in SessionGameParametersContainer) AddGroupedParamsToDictionary(container, _allParameters);

        foreach (var param in SettingsParameters.Parameters)
        {
            string lowerKey = param.Key.ToLower();
            if (_allParameters.ContainsKey(lowerKey))
            {
                UnityEngine.Debug.LogError($"[GameData] Дубликат ключа '{lowerKey}'!");
                continue;
            }
            _allParameters.Add(lowerKey, param.Value);
        }



    }

    private void AddGroupedParamsToDictionary(GameParametersData container, Dictionary<string, IGameParameter> dictionary)
    {
        foreach (var param in container.Parameters)
        {
            string lowerKey = param.Key.ToLower();
            if (dictionary.ContainsKey(lowerKey))
            {
                UnityEngine.Debug.LogError($"[GameData] Дубликат ключа '{lowerKey}'!");
                continue;
            }
            dictionary.Add(lowerKey, param.Value);
        }
    }


    public bool TrySetParam(string key, string textValue, out string finalValue)
    {
        finalValue = string.Empty;
        string lowerKey = key.ToLower();

        if (_allParameters.TryGetValue(lowerKey, out var param))
        {
            if (!param.IsValueValid(textValue)) return false;

            param.Set(textValue);
            finalValue = param.AsString();
            return true;
        }

        return false;
    }


    public bool TryGetParam(string key, out string finalValue, out string typeInfo)
    {
        finalValue = string.Empty;
        typeInfo = string.Empty;
        string lowerKey = key.ToLower();

        if (_allParameters.TryGetValue(lowerKey, out var param))
        {
            finalValue = param.AsString();
            typeInfo = param.GetType().Name.Replace("GameParameter", "");
            return true;
        }
        return false;
    }


    public void ResetSessionDataToDefaults()
    {
        foreach (var param in SessionParameters)
        {
            param.Value.ResetToDefault();
        }

        foreach (var complexContainer in SessionClassesData)
        {
            complexContainer.ResetToDefaults();
        }
    }

    public bool HasParam(string key) => _allParameters.ContainsKey(key.ToLower());
    public IEnumerable<string> GetAllParamNames() => _allParameters.Keys;
}
