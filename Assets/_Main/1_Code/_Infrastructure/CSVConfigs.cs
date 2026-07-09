using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable]
public class CSVConfigs
{
    [SerializeField] private TextAsset _tasksCSV;
    [SerializeField] private TextAsset _gameplayTimeShceduleCSV;
    public List<TaskExample> TasksConfig { get; private set; }
    public List<DayTime> DaysTimeConfig { get; private set; }

    public void Startup()
    {
        TasksConfig = new List<TaskExample>();
        DaysTimeConfig = new List<DayTime>();

        ParseGameplayTimeCSVConfig(_gameplayTimeShceduleCSV);
        ParseTasksCSVConfig(_tasksCSV);

    }


    public void ParseGameplayTimeCSVConfig(TextAsset csvFile)
    {
        if (csvFile == null)
        {
            Debug.LogError("[ScheduleDataProvider] CSV файл расписания пустой или не передан!");
            return;
        }

        string cleanText = csvFile.text.Replace("\r", "");
        string[] lines = cleanText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1) return;

        // Начинаем со строки 1 (пропуская заголовки)
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length < 4) continue;

            try
            {
                // Читаем строго по индексам из вашей таблицы:
                // 0 = LevelIndex, 1 = Name, 2 = StartTime ("09:00"), 3 = EndTime ("22:00")
                DayTime level = new DayTime(
                    levelIndex: int.Parse(columns[0].Trim(), CultureInfo.InvariantCulture),
                    name: columns[1].Trim(),
                    timeStart: TimeSpan.Parse(columns[2].Trim(), CultureInfo.InvariantCulture), // Авто-парсинг "09:00"
                    timeEnd: TimeSpan.Parse(columns[3].Trim(), CultureInfo.InvariantCulture)    // Авто-парсинг "22:00"
                );

                DaysTimeConfig.Add(level);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ScheduleDataProvider] Ошибка парсинга расписания на строке {i}: {e.Message}");
            }
        }
    }
    public void ParseTasksCSVConfig(TextAsset csvFile)
    {
        if (csvFile == null)
        {
            Debug.LogError("[TaskDataProvider] CSV файл пустой или не передан!");
            return;
        }

        string cleanText = csvFile.text.Replace("\r", "");
        string[] lines = cleanText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1) return;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length < 3) continue;

            try
            {
                TaskExample task = new TaskExample(
                    taskID: columns[0].Trim(),
                    day: int.Parse(columns[1].Trim(), CultureInfo.InvariantCulture),
                    timeLimit: float.Parse(columns[2].Trim(), CultureInfo.InvariantCulture)
                );

                TasksConfig.Add(task);
            }
            catch (Exception e)
            {
                Debug.LogError($"[TaskDataProvider] Ошибка парсинга строки {i} в файле {csvFile.name}: {e.Message}");
            }
        }
    }

}

