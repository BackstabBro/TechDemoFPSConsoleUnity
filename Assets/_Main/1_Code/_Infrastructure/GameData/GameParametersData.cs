using System;
using System.Collections.Generic;

public abstract class GameParametersData 
{
    public string DisplayName { get; protected set; }
    public Dictionary<string, IGameParameter> Parameters { get; protected set; } = new();

}

