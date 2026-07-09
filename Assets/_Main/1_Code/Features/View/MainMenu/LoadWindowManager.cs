using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

public class LoadWindowManager : MonoBehaviour
{
    public event Action OnGameLoaded;

    [SerializeField] private SaveSlotUiButton[] saveSlotUiButtons;
    [SerializeField] private RectTransform ChooseActionWindow;
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button negativeButton;
    [SerializeField] private Button backButtonFromLoadMenu;

    private SaveLoadService _saveLoadService;
    private GameStateService _gameStateService;

    private int selectedSlotIndex = -1;
    private InputService _inputService;

    [Inject]
    public void Construct(SaveLoadService saveLoadService, GameStateService gameStateService, InputService inputService)
    {
        _saveLoadService = saveLoadService;
        _gameStateService = gameStateService;
        _inputService = inputService;
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

        backButton.onClick.AddListener(Close);
        confirmButton.onClick.AddListener(HandleConfirmButtonClick);
        negativeButton.onClick.AddListener(DeleteGame);
        backButtonFromLoadMenu.onClick.AddListener(CloseChooseActionWindow);
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
        backButton.gameObject.SetActive(true);
        _gameStateService.PushState(GameStates.Pause);
    }

    public void Close()
    {
        backButton.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
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



    private void OpenChooseActionWindow()
    {
        ChooseActionWindow.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
        backButton.gameObject.SetActive(false);

    }


    private void HandleConfirmButtonClick()
    {
        _saveLoadService.SetCurrentSlot(selectedSlotIndex);
        if (_saveLoadService.LoadGame())
        {
            Close();
            OnGameLoaded?.Invoke();
        }
        else
        {
            CloseChooseActionWindow();
        }
    }


    private void DeleteGame()
    {
        _saveLoadService.SetCurrentSlot(selectedSlotIndex);
        _saveLoadService.DeleteSaveSlotData();
        saveSlotUiButtons[selectedSlotIndex].text.text = $"{selectedSlotIndex} EMPTY";

        foreach (var button in saveSlotUiButtons)
        {
            button.gameObject.SetActive(true);
            button.ResetColors();
        }

        CloseChooseActionWindow();
    }

    private void CloseChooseActionWindow()
    {
        foreach (var button in saveSlotUiButtons)
        {
            button.gameObject.SetActive(true);
            button.ResetColors();
        }

        ChooseActionWindow.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        backButton.gameObject.SetActive(true);
    }




    public void CleanUp()
    {
        for (int i = 0; i < saveSlotUiButtons.Length; i++)
        {
            saveSlotUiButtons[i].OnClick -= HandleSaveSlotClick;
        }

        backButton.onClick.RemoveListener(Close);
        confirmButton.onClick.RemoveListener(HandleConfirmButtonClick);
        negativeButton.onClick.RemoveListener(DeleteGame);
        backButtonFromLoadMenu.onClick.RemoveListener(CloseChooseActionWindow);
    }

}
