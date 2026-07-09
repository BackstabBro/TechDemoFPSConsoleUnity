using System;
using System.Collections.Generic;

public class TimeParameters : GameParametersData
{
    public IGameParameter<int> CurrentLevel => _currentLevel;
    public IGameParameter<string> CurrentLevelName => _currentLevelName;
    public IGameParameter<TimeSpan> CurrentTime => _currentTime;
    public IGameParameter<bool> IsReachedTodayTimeLimit => _isReachedTodayTimeLimit;
    public IGameParameter<float> RealSecondsForGameMinute => _realSecondsForGameMinute;

    private readonly GameParameterInt _currentLevel = new("current_level", 1, 1, 7);
    private readonly GameParameterString _currentLevelName = new("day_name", "");
    private readonly GameParameterTimeSpan _currentTime = new("current_time", TimeSpan.MinValue, null, null);
    private readonly GameParameterBool _isReachedTodayTimeLimit = new("day_limit_reached", false);
    private readonly GameParameterFloat _realSecondsForGameMinute = new("seconds_to_game_minutes", 3.5f, 1f, 100f);

    public TimeParameters()
    {
        DisplayName = "TimeData";

        Parameters = new Dictionary<string, IGameParameter>
        {
            { _currentLevel.Name, _currentLevel },
            { _currentLevelName.Name, _currentLevelName },
            { _currentTime.Name, _currentTime },
            { _isReachedTodayTimeLimit.Name, _isReachedTodayTimeLimit },
            { _realSecondsForGameMinute.Name, _realSecondsForGameMinute }
        };
    }

}
