using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveFile
{
    public DateTime SaveTime { get; set; } = DateTime.Now;
    public Dictionary<string, string> GameParams { get; set; } = new();
    public Dictionary<string, object> GameClassesData { get; set; } = new();


    public bool IsValid()
    {
        if (GameParams != null && GameParams.Count > 0 && GameClassesData != null && GameClassesData.Count > 0) return true;
        return false;
    }
}
