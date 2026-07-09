using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class DayTimeService : IDisposable
{
    public event Action OnWorkingTimeOver;

    private IGameParameter<int> _currentLevelIndex;
    private IGameParameter<string> _currentDayName;
    private IGameParameter<TimeSpan> _currentTime;
    private IGameParameter<bool> _isReachedTodayTimeLimit;
    private IGameParameter<float> _realSecondsForGameMinute;
    
    private float timeMultiplier;
    private TimeSpan _currentDayEndTime;

    private readonly GameData _gameData;
    private readonly CSVConfigs _CSVConfigRegistry;

    private readonly SaveLoadService _saveLoadService;

    private IReadOnlyList<DayTime> _levels;

    [Inject]
    public DayTimeService(GameData gameData, CSVConfigs cSVConfigsRegistry, SaveLoadService saveLoadService)
    {
        _gameData = gameData;
        _saveLoadService = saveLoadService;
        _CSVConfigRegistry = cSVConfigsRegistry;

        _currentLevelIndex = _gameData.TimeParameters.CurrentLevel;
        _currentDayName = _gameData.TimeParameters.CurrentLevelName;
        _currentTime = _gameData.TimeParameters.CurrentTime;
        _isReachedTodayTimeLimit = _gameData.TimeParameters.IsReachedTodayTimeLimit;
        _realSecondsForGameMinute = _gameData.TimeParameters.RealSecondsForGameMinute;
    }

    public void Init()
    {
        _currentLevelIndex.OnChanged += ChangeDay;
        _levels = _CSVConfigRegistry.DaysTimeConfig;
        timeMultiplier = 60f / _realSecondsForGameMinute.Value;        
    }


    public void Startup()
    {
        DayTime currentDay = null;

        for (int i = 0; i < _levels.Count; i++)
        {
            if (_levels[i].LevelIndex == _currentLevelIndex.Value)
            {
                currentDay = _levels[i];
                break;
            }
        }

        if (currentDay == null)
        {
            Debug.LogError($"[GameplayTimeService] Расписание для уровня {_currentLevelIndex.Value} не найдено!");
            return;
        }

        _currentDayName.Set(currentDay.Name);
        _currentTime.Set(currentDay.TimeStart);
        _currentDayEndTime = currentDay.TimeEnd;

        _isReachedTodayTimeLimit.Set(false);

        Debug.Log($"[GameplayTimeService] Уровень {_currentLevelIndex.Value} ({_currentDayName.Value}) запущен!");
    }

    private void ChangeDay(IGameParameter obj)
    {
        OnWorkingTimeOver?.Invoke();
        SceneManager.LoadScene(1);
    }

    public void TickGameTime()
    {
        if (_isReachedTodayTimeLimit.Value) return;
        
        double gameSecondsPassed = Time.deltaTime * timeMultiplier;
        _currentTime.Set(_currentTime.Value.Add(TimeSpan.FromSeconds(gameSecondsPassed)));

        if (_currentTime.Value >= _currentDayEndTime)
        {
            _currentTime.Set(_currentDayEndTime);
            _isReachedTodayTimeLimit.Set(true);
            OnWorkingTimeOver?.Invoke();
            Debug.Log($"[GameplayTimeService] Рабочее время уровня вышло! На часах: {_currentTime.Value.ToString(@"hh\:mm")}.");
        }
    }

    public void GoToNextDay()
    {
        int nextLevelIndex = _currentLevelIndex.Value + 1;

        if (nextLevelIndex >= _levels.Count) { return; }

        Debug.Log($"[GameplayTimeService] Игрок уснул. Перезагрузка сцены для Дня {nextLevelIndex}...");
        _currentLevelIndex.Set(nextLevelIndex);
    }

    public void Dispose()
    {
        _currentLevelIndex.OnChanged -= ChangeDay;
    }
}
