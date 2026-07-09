using System;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class MainMenuBootstrapper : IInitializable, IStartable, IDisposable
{
    private readonly MainMenuManager _mainMenuManager;
    private readonly LoadWindowManager _loadWindowManager;


    [Inject]
    public MainMenuBootstrapper (MainMenuManager mainMenuManager, LoadWindowManager loadWindowManager)
    {
        _mainMenuManager = mainMenuManager;
        _loadWindowManager = loadWindowManager;

        _loadWindowManager.OnGameLoaded += StartGame;
    }

    public void Initialize()
    {
        
    }

    public void Start()
    {
        _mainMenuManager.Init();
        _loadWindowManager.Init();
        _loadWindowManager.Startup();
    }


    private void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void Dispose()
    {
        _mainMenuManager.Cleanup();
        _loadWindowManager.CleanUp();
    }
}
