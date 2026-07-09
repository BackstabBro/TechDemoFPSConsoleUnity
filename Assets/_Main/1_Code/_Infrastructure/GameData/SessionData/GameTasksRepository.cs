using Newtonsoft.Json;
using UnityEngine;
using VContainer;

public class GameTasksRepository : PersistentList<TaskExample>
{
    public override string Name => "GameTasksData";

    private readonly CSVConfigs _csvConfigs;

    [Inject]
    public GameTasksRepository(CSVConfigs csvConfigs)
    {       
        _csvConfigs = csvConfigs;
    }

    public void SetDefaultValues()
    {
        Data.Clear();

        if (_csvConfigs.TasksConfig == null)
        {
            Debug.LogError("[GameTasksRepository] Конфиг заданий в CSVConfigs пуст! Инициализация прервана.");
            return;
        }

        foreach (var task in _csvConfigs.TasksConfig)
        {
            TaskExample newGameTaskInstance = new(task.ID, task.Day, task.TimeLimit);
            Data.Add(newGameTaskInstance);
        }

        Debug.Log($"[GameTasksRepository] Успешно инициализировано {Data.Count} дефолтных заданий из CSV.");
    }

    public override void ResetToDefaults()
    {
        SetDefaultValues();
    }

}

public class TaskExample
{
    public string ID { get; private set; }
    public int Day { get; private set; }
    public float TimeLimit;
    public TaskState State;

    public TaskExample(string taskID, int day, float timeLimit)
    {
        ID = taskID;
        Day = day;
        TimeLimit = timeLimit;
        State = TaskState.Unavailable;
    }

    [JsonConstructor]
    public TaskExample(string ID, int Day, float TimeLimit, TaskState State)
    {
        this.ID = ID;
        this.Day = Day;
        this.TimeLimit = TimeLimit;
        this.State = State;
    }
}

public enum TaskState
{
    Unavailable = 0,
    Available = 1,
    Active = 2,
    Completed = 3,
    Failed = 4
}