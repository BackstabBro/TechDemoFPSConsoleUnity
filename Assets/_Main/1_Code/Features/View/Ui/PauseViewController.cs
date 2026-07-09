using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

public class PauseViewController : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _closeGameButton;
    [SerializeField] private Image _reticle;

    private PauseService _pauseService;


    [Inject]
    public void Construct(PauseService pauseService)
    {
        _pauseService = pauseService;

    }

    public void Init()
    {
        _pauseService.OnPauseActive += HandleShowPauseMenu;
        _pauseService.OnShowPauseDeathScreen += HandleShowPauseDeathMenu;

        _continueButton.onClick.AddListener(_pauseService.QuitPause);
        _restartButton.onClick.AddListener(_pauseService.RestartGame);
        _closeGameButton.onClick.AddListener(_pauseService.QuitGame);
    }


    public void Startup()
    {
        gameObject.SetActive(false);
    }

    private void HandleShowPauseDeathMenu()
    {
        gameObject.SetActive(true);
        _continueButton.interactable = false;
    }




    private void HandleShowPauseMenu(bool isActive)
    {
        gameObject.SetActive(isActive);

        if (isActive) 
        {
            EventSystem.current.SetSelectedGameObject(null);
            _reticle.gameObject.SetActive(false);
        }
        else
        {
            _reticle.gameObject.SetActive(true);
        }
    }


    public void CleanUp()
    {
        _pauseService.OnPauseActive -= HandleShowPauseMenu;
        _continueButton.onClick.RemoveAllListeners();
        _restartButton.onClick.RemoveAllListeners();
        _closeGameButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {

    }
}
