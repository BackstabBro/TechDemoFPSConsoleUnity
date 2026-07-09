using System;

public class DayTime
{
    public int LevelIndex { get; }
    public string Name { get; }
    public TimeSpan TimeStart { get; } // Хранит время в формате Часы:Минуты:Секунды
    public TimeSpan TimeEnd { get; }

    public DayTime(int levelIndex, string name, TimeSpan timeStart, TimeSpan timeEnd)
    {
        LevelIndex = levelIndex;
        Name = name;
        TimeStart = timeStart;
        TimeEnd = timeEnd;
    }
}
