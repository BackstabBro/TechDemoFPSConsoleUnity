using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;


    private SaveLoadService _saveLoadService;
    private LoadWindowManager _loadWindowManager;

    [Inject]
    public void Construct(SaveLoadService saveLoadService, LoadWindowManager loadWindowManager)
    {
        _saveLoadService = saveLoadService;
        _loadWindowManager = loadWindowManager;
    }

    public void Init()
    {
        newGameButton.onClick.AddListener(StartNewGame);
        loadGameButton.onClick.AddListener(OpenLoadMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        quitButton.onClick.AddListener(QuitGame);
    }


    private void StartNewGame()
    {
        _saveLoadService.SetupNewGame(-1);
        SceneManager.LoadScene(2);
    }


    private void OpenLoadMenu()
    {
        _loadWindowManager.Open();
    }

    private void OpenSettingsMenu()
    {

    }


    private void QuitGame()
    {
        Application.Quit();
    }

    public void Cleanup()
    {
        newGameButton.onClick.RemoveListener(StartNewGame);
        loadGameButton.onClick.RemoveListener(OpenLoadMenu);
        settingsButton.onClick.RemoveListener(OpenSettingsMenu);
        quitButton.onClick.RemoveListener(QuitGame);
    }

}
