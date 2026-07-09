
public interface ISaveableData
{
    string Name { get; }
    void SaveData(GameSaveFile saveFile); // Метод сам запишет свои данные в сейв
    void LoadData(GameSaveFile saveFile); // Метод сам прочитает свои данные из сейва
    void ResetToDefaults();
}
