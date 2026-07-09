using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using VContainer;

public class SaveLoadService : IDisposable
{
    public event Action OnSaveDataUpdated;

    public int CurrentSlotIndex { get; private set; } = -1;
    private readonly GameData _gameData;
    private readonly string _saveFolderPath;

    private SaveSlot[] _saveSlots = new SaveSlot[7];
    private JsonSerializerSettings saveSettings;
    private JsonSerializerSettings loadSettings;


    [Inject]
    public SaveLoadService(GameData gameData)
    {
        _gameData = gameData;
        _saveFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
    }

    public void Init()
    {
        saveSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        loadSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

        if (!Directory.Exists(_saveFolderPath))
        {
            Directory.CreateDirectory(_saveFolderPath);
        }

        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i] = new SaveSlot(i);
            UpdateSaveSlotData(i);
        }

        foreach (var kvp in _gameData.SettingsParameters.Parameters)
        {
            kvp.Value.OnChanged += SettingsChanged;
        }
    }
        

    public void SetCurrentSlot(int slotIndex)
    {
        if (slotIndex < -1) slotIndex = -1;
        if (slotIndex > _saveSlots.Length - 1) slotIndex = _saveSlots.Length - 1;
        CurrentSlotIndex = slotIndex;
    }
        

    public void SetupNewGame(int slotIndex)
    {
        CurrentSlotIndex = slotIndex;
        _gameData.ResetSessionDataToDefaults();
    }

    public void SaveCurrentGame()
    {
        if (CurrentSlotIndex == -1)
        {
            Debug.LogError("[SaveSystem] Невозможно сохранить игру! Активный слот не выбран.");
            return;
        }

        GameSaveFile saveFile = new GameSaveFile();

        saveFile.SaveTime = DateTime.Now;

        foreach (var kvp in _gameData.SessionParameters)
        {
            saveFile.GameParams.Add(kvp.Key, kvp.Value.AsString());
        }

        foreach (var container in _gameData.SessionClassesData)
        {
            container.SaveData(saveFile);
        }

        WriteGameSaveFileWithBackup(saveFile, $"save_slot_{CurrentSlotIndex}.json",saveSettings);

        Debug.Log($"[SaveSystem] Сохранение в текущий слот {CurrentSlotIndex} успешно выполнено.");

        UpdateSaveSlotData(CurrentSlotIndex);
        OnSaveDataUpdated?.Invoke();
    }


    private void UpdateSaveSlotData(int index)
    {
        if (index < 0 || index >= _saveSlots.Length)
        {
            Debug.LogError($"[SaveSystem] Попытка обновить слот с неверным индексом: {index}");
            return;
        }

        SaveSlot slot = _saveSlots[index];

        string filePath = Path.Combine(_saveFolderPath, $"save_slot_{index}.json");

        if (!File.Exists(filePath))
        {
            slot.Reset();
            Debug.Log($"[SaveSystem] Слот {index}: файл не найден, данные сброшены.");
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            GameSaveFile saveFile = JsonConvert.DeserializeObject<GameSaveFile>(json);

            if (saveFile?.GameParams != null && saveFile.GameParams.TryGetValue("current_level", out var currentLevel))
            {
                slot.CurrentDay = currentLevel;
                slot.SaveTime = saveFile.SaveTime;
                slot.IsEmpty = false;
                Debug.Log($"[SaveSystem] Слот {index} успешно обновлен. День/Уровень: {slot.CurrentDay}, Время: {slot.SaveTime}");
            }
            else
            {
                Debug.LogWarning($"[SaveSystem] Слот {index}: файл поврежден или отсутствует ключ.");
                slot.Reset();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Критическая ошибка при чтении слота {index}: {e.Message}");
            slot.Reset();
        }        
    }

    public bool LoadGame()
    {
        string filePath = Path.Combine(_saveFolderPath, $"save_slot_{CurrentSlotIndex}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[SaveSystem] Файл сохранения {filePath} не найден!");
            return false;
        }

        string json = File.ReadAllText(filePath);
        GameSaveFile saveFile = JsonConvert.DeserializeObject<GameSaveFile>(json, loadSettings);


        if (saveFile  == null || !saveFile.IsValid())
        {
            Debug.LogError($"[SaveSystem] Файл сохранения {filePath} поврежден, пуст или некорректен!");
            return false;
        }      

        foreach (var kvp in saveFile.GameParams)
        {
            string lowerKey = kvp.Key.ToLower();
            if (_gameData.SessionParameters.TryGetValue(lowerKey, out var param))
            {
                param.Set(kvp.Value);
            }
        }

        foreach (var container in _gameData.SessionClassesData)
        {
            container.LoadData(saveFile);
        }

        Debug.Log($"[SaveSystem] Слот {CurrentSlotIndex} успешно загружен и применен к GameData.");
        return true;
    }


    public void SaveSettings()
    {
        GameSaveFile profile = new GameSaveFile();

        foreach (var kvp in _gameData.SettingsParameters.Parameters)
        {
            profile.GameParams.Add(kvp.Key.ToLower(), kvp.Value.AsString());
        }

        WriteGameSaveFileWithBackup(profile, "settings.json");
        Debug.Log("[SaveSystem] Глобальные настройки (камера/звук) сохранены на диск.");
    }



    public void LoadSettings()
    {
        string filePath = Path.Combine(_saveFolderPath, "settings.json");
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);

        GameSaveFile profile = JsonConvert.DeserializeObject<GameSaveFile>(json);

        if (profile?.GameParams == null || profile.GameParams.Count == 0)
        {
            Debug.LogError($"[SaveSystem] Файл настроек пустой!");
            return;
        }

        foreach (var kvp in profile.GameParams)
        {
            string lowerKey = kvp.Key.ToLower();

            if (_gameData.SettingsParameters.Parameters.TryGetValue(lowerKey, out var param))
            {
                param.Set(kvp.Value);
            }
        }

        Debug.Log("[SaveSystem] Глобальные настройки успешно применены при старте игры.");
    }

    public bool DeleteSaveSlotData()
    {
        string filePath = Path.Combine(_saveFolderPath, $"save_slot_{CurrentSlotIndex}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[SaveSystem] Попытка удалить несуществующий слот {CurrentSlotIndex}.");
            return false;
        }

        if (CurrentSlotIndex < 0 || CurrentSlotIndex >= _saveSlots.Length)
        {
            Debug.LogError($"[SaveSystem] Неверный индекс слота для удаления: {CurrentSlotIndex}");
            return false;
        }

        try
        {
            File.Delete(filePath);

            _saveSlots[CurrentSlotIndex].Reset();

            _gameData.ResetSessionDataToDefaults();

            Debug.Log($"[SaveSystem] Слот сохранения {CurrentSlotIndex} успешно удален.");          

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Критическая ошибка при удалении слота {CurrentSlotIndex}: {e.Message}");
            return false;
        }
    }

    private void WriteGameSaveFileWithBackup(GameSaveFile profile, string fileName, JsonSerializerSettings settings = null)
    {
        string finalPath = Path.Combine(_saveFolderPath, fileName);
        string tempPath = finalPath + ".tmp";

        try
        {
            string json = settings != null
            ? JsonConvert.SerializeObject(profile, settings)
            : JsonConvert.SerializeObject(profile, Formatting.Indented);

            File.WriteAllText(tempPath, json);

            if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }
            File.Move(tempPath, finalPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Критическая ошибка при записи файла {fileName}: {e.Message}");
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    private void SettingsChanged(IGameParameter parameter)
    {
        SaveSettings();
    }

    public SaveSlot GetSaveSlot(int index)
    {
        if (index < 0) index = 0;
        if (index > _saveSlots.Length - 1) index = _saveSlots.Length - 1;

        return _saveSlots[index];
    }


    public bool DoesSaveExist(int slotIndex)
    {
        return File.Exists(Path.Combine(_saveFolderPath, $"save_slot_{slotIndex}.json"));
    }



    public void Dispose()
    {
        if (_gameData?.SettingsParameters?.Parameters != null)
        {
            foreach (var kvp in _gameData.SettingsParameters.Parameters)
            {
                kvp.Value.OnChanged -= SettingsChanged;
            }
        }

    }
}
