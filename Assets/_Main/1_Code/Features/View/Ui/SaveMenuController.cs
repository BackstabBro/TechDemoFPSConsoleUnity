using UnityEngine;
using UnityEngine.UI;
using VContainer;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SaveMenuController : MonoBehaviour
{
    [SerializeField] private SaveSlotUiButton[] saveSlotUiButtons;
    [SerializeField] private RectTransform ChooseActionWindow;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;

    private SaveLoadService _saveLoadService;
    private GameStateService _gameStateService;

    private int selectedSlotIndex = -1;


    [Inject]
    public void Construct(SaveLoadService saveLoadService, GameStateService gameStateService)
    {
        _saveLoadService = saveLoadService;
        _gameStateService = gameStateService;
        _saveLoadService.OnSaveDataUpdated += UpdateSaveInfo;
    }

    public void Init()
    {
        for (int i = 0; i < saveSlotUiButtons.Length; i++)
        {
            saveSlotUiButtons[i].SetIndex(i);
            saveSlotUiButtons[i].OnClick += HandleSaveSlotClick;

            SaveSlot saveData = _saveLoadService.GetSaveSlot(i);


            if (saveData.IsEmpty)
            {
                saveSlotUiButtons[i].text.text = $"{saveData.Index} EMPTY";
            }
            else
            {
                saveSlotUiButtons[i].text.text = $"{saveData.Index} DAY {saveData.CurrentDay} {saveData.SaveTime}";
            }

        }

        confirmButton.onClick.AddListener(HandleConfirmButtonClick);
        backButton.onClick.AddListener(HandleBackButtonClick);
    }

    public void Startup()
    {
        ChooseActionWindow.gameObject.SetActive(false);
        gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        _gameStateService.PushState(GameStates.Pause);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
        SceneManager.LoadScene(2);
    }

    private void UpdateSaveInfo()
    {
        for (int i = 0; i < saveSlotUiButtons.Length; i++)
        {
            SaveSlot saveData = _saveLoadService.GetSaveSlot(i);


            if (saveData.IsEmpty)
            {
                saveSlotUiButtons[i].text.text = $"{saveData.Index} EMPTY";
            }
            else
            {
                saveSlotUiButtons[i].text.text = $"{saveData.Index} DAY {saveData.CurrentDay} {saveData.SaveTime}";
            }

        }
    }



    private void HandleSaveSlotClick(int index)
    {
        foreach (var button in saveSlotUiButtons)
        {
            button.gameObject.SetActive(false);
        }

        saveSlotUiButtons[index].gameObject.SetActive(true);

        selectedSlotIndex = index;


        OpenChooseActionWindow();
    }

    private void HandleConfirmButtonClick ()
    {
        _saveLoadService.SetCurrentSlot(selectedSlotIndex);
        _saveLoadService.SaveCurrentGame();
        _gameStateService.PushState(GameStates.Gameplay);
        Close();

    }

    private void HandleBackButtonClick()
    {
        selectedSlotIndex = -1;
        _gameStateService.PushState(GameStates.Gameplay);
        Close();
    }

    private void OpenChooseActionWindow()
    {
        ChooseActionWindow.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);

    }

    private void CloseChooseActionWindow()
    {
        foreach (var button in saveSlotUiButtons)
        {
            button.gameObject.SetActive(true);
            button.ResetColors();
        }

        EventSystem.current.SetSelectedGameObject(null);
        ChooseActionWindow.gameObject.SetActive(false);
    }


    public void CleanUp()
    {
        _saveLoadService.OnSaveDataUpdated -= UpdateSaveInfo;

        for (int i = 0; i < saveSlotUiButtons.Length; i++)
        {
            saveSlotUiButtons[i].OnClick -= HandleSaveSlotClick;
        }

        confirmButton.onClick.RemoveListener(HandleConfirmButtonClick);
        backButton.onClick.RemoveListener(HandleBackButtonClick);
    }

}
