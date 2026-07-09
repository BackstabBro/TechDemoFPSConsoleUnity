using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class TaskService : IDisposable
{
    public Dictionary<string, TaskExample> Tasks { get; private set; }
    public List<TaskExample> ActiveTasks { get; private set; }

    public event Action OnTasksChanged;

    private IGameParameter<int> _currentLevelIndex;

    private List<TaskExample> _сachedTasks;
    private readonly DayTimeService _gameplayTimeService;
    private readonly GameData _gameData;

    [Inject]
    public TaskService(
        DayTimeService gameplayTimeService,
        GameData gameData)
    {
        _gameData = gameData;
        _currentLevelIndex = _gameData.TimeParameters.CurrentLevel;
        _gameplayTimeService = gameplayTimeService;
    }

    public void Init()
    {
        _gameplayTimeService.OnWorkingTimeOver += BlockAvaibleTasks;

        _сachedTasks = _gameData.GameTasksData.Data;
        Tasks = new Dictionary<string, TaskExample>();
        ActiveTasks = new List<TaskExample>();

        foreach (var task in _сachedTasks)
        {
            Tasks.TryAdd(task.ID, task);

            if (task.State == TaskState.Active)
            {
                ActiveTasks.Add(task);
            }
        }


        Debug.Log($"[{GetType().Name}] init complete");
    }

    public void Startup()
    {
        SetupAvaibleTasks(_currentLevelIndex.Value);
    }


    public void SetupAvaibleTasks(int currentDay)
    {
        bool hasChanges = false;

        foreach (var key in Tasks)
        {
            var task = key.Value;

            if (task.Day == currentDay && task.State == TaskState.Unavailable)
            {
                task.State = TaskState.Available;
                hasChanges = true;
                Debug.Log($"[TaskService] Задание {task.ID} теперь доступно для Дня {currentDay}");
            }
        }

        if (hasChanges) OnTasksChanged?.Invoke();
    }



    public void AcceptTask(string taskId)
    {
        if (!Tasks.TryGetValue(taskId, out var task)) return;

        if (task.State == TaskState.Available)
        {
            task.State = TaskState.Active;
            ActiveTasks.Add(task);
            OnTasksChanged?.Invoke();
            Debug.Log($"[TaskService] Задание {taskId} принято в работу.");
        }
    }

    public void UpdateActiveTimers(float deltaTime)
    {
        int count = ActiveTasks.Count;
        if (count == 0) return;

        bool hasChanges = false;

        for (int i = ActiveTasks.Count - 1; i >= 0; i--)
        {
            var task = ActiveTasks[i];
            task.TimeLimit -= deltaTime;

            if (task.TimeLimit <= 0)
            {
                task.TimeLimit = 0;
                task.State = TaskState.Failed;

                Debug.Log($"[TaskService] Время на задание {task.ID} вышло!");

                ActiveTasks.RemoveAt(i);

                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            OnTasksChanged?.Invoke();
        }
    }


    public bool IsTaskAvailable(string taskId)
    {
        if (Tasks.TryGetValue(taskId,out var task) && task.State == TaskState.Available) return true;
        else return false;
    } 



    public bool HasActiveTask(string taskId)
    {
        if (Tasks.TryGetValue(taskId, out var task) && task.State == TaskState.Active) return true;
        else return false;
    }

    public void CompleteTask(string taskId)
    {
        if (!Tasks.TryGetValue(taskId, out var task)) return;

        if (task.State == TaskState.Active)
        {
            ActiveTasks.Remove(task);
        }

        task.State = TaskState.Completed;
        OnTasksChanged?.Invoke();
        Debug.Log($"[TaskService] Задание {taskId} успешно выполнено!");

    }

    public void FailTask(string taskId)
    {
        if (!Tasks.TryGetValue(taskId, out var task)) return;

        if (task.State == TaskState.Active)
        {
            ActiveTasks.Remove(task);
        }

        task.State = TaskState.Failed;
        OnTasksChanged?.Invoke();
        Debug.Log($"[TaskService] Задание {taskId} провалено.");
    }

    private void BlockAvaibleTasks()
    {
        bool hasChanges = false;

        foreach (var task in Tasks)
        {
            if (task.Value.State == TaskState.Available)
            {
                task.Value.State = TaskState.Unavailable;
                hasChanges = true;
            }
        }
        if (hasChanges) OnTasksChanged?.Invoke();
    }

    public void FailAllActiveTasksOnDayEnd()
    {
        if (ActiveTasks == null || ActiveTasks.Count == 0) return;

        Debug.Log($"[TaskService] Конец дня. Автоматически провалено {ActiveTasks.Count} незавершенных заданий.");

        for (int i = ActiveTasks.Count - 1; i >= 0; i--)
        {
            var task = ActiveTasks[i];

            task.State = TaskState.Failed;
            task.TimeLimit = 0;
        }

        ActiveTasks.Clear();
        OnTasksChanged?.Invoke();
    }

    public void Dispose()
    {
        _gameplayTimeService.OnWorkingTimeOver -= BlockAvaibleTasks;
        FailAllActiveTasksOnDayEnd();
    }

}
