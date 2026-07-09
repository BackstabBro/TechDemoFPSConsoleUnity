using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public abstract class PersistentList<T> : ISaveableData
{
    public abstract string Name { get; }
    public List<T> Data { get; private set; } = new();

    public void SaveData(GameSaveFile saveFile)
    {
        saveFile.GameClassesData[Name.ToLower()] = JArray.FromObject(Data);
    }

    public void LoadData(GameSaveFile saveFile)
    {
        string key = Name.ToLower();
        if (saveFile.GameClassesData.TryGetValue(key, out object savedData))
        {
            string rawJson = savedData.ToString();
            List<T> loadedItems = JsonConvert.DeserializeObject<List<T>>(rawJson);

            if (loadedItems != null)
            {
                Data.Clear(); 
                Data.AddRange(loadedItems); 
                UnityEngine.Debug.Log($"[{Name}] Успешно загружено {Data.Count} элементов.");
            }
        }
    }


    public abstract void ResetToDefaults();
}
