using System;

public class SaveSlot
{
    public int Index {  get; private set; }
    public string CurrentDay { get;  set; }
    public DateTime SaveTime { get; set; }
    public bool IsEmpty { get; set; } = true;

    public SaveSlot (int index)
    {
        Index = index;
    }


    public void Reset()
    {
        SaveTime = default;
        IsEmpty = true;
    }
}
