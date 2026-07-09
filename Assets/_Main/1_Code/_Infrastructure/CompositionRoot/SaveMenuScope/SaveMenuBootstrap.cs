using System;
using UnityEngine;
using VContainer.Unity;

public class SaveMenuBootstrap : IStartable, IDisposable
{
    private readonly SaveMenuController _saveMenuController;

    public SaveMenuBootstrap(SaveMenuController saveMenuController)
    {
        _saveMenuController = saveMenuController;
    }

    public void Start()
    {
        _saveMenuController.Init();
        _saveMenuController.Startup();
        _saveMenuController.Open();
    }

    public void Dispose()
    {
        _saveMenuController.CleanUp();
    }
}
