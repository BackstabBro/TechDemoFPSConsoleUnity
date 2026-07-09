using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public class TasksDebugMenu : MonoBehaviour 
{
    [SerializeField] private TaskAcceptButton _buttonPrefab;
    [SerializeField] private RectTransform _spawnAvaibleTasks;
    [SerializeField] private TMP_Text _allTasksText;

    private TaskService _tasksService;
    private InputService _inputService;
    private GameStateService _gameStateService;

    bool isActive = false;
    private int poolCapacity = 20;

    private List<TaskAcceptButton> _spawnedButtons;

    [Inject]
    public void Construct(InputService inputService, GameStateService gameStateService, TaskService tasksService)
    {
        _inputService = inputService;
        _tasksService = tasksService;
        _gameStateService = gameStateService;
    }

    public void Init()
    {
        _inputService.OnShowTasksMenu += OpenClose;
        _tasksService.OnTasksChanged += UpdateView;
    }

    public void Startup()
    {
        _spawnedButtons = new List<TaskAcceptButton>(poolCapacity);

        for (int i = 0; i < poolCapacity; i++)
        {
            TaskAcceptButton buttonInstance = Instantiate(_buttonPrefab, _spawnAvaibleTasks);
            buttonInstance.Init();
            buttonInstance.OnClick += HandleButton_OnClick;
            _spawnedButtons.Add(buttonInstance);
        }
        gameObject.SetActive(false);
    }



    private void HandleButton_OnClick(TaskExample taskData)
    {
        _tasksService.AcceptTask(taskData.ID);
        EventSystem.current.SetSelectedGameObject(null);
    }



    private void OpenClose()
    {
        isActive = !isActive;
        gameObject.SetActive(isActive);

        if (isActive)
        {
            _gameStateService.PushState(GameStates.Menu);
            UpdateView();
        }
        else
        {
            _gameStateService.PopState();
        }
    }





    private void UpdateView()
    {
        if (!isActive) return;

        UpdateTasksText();

        List<TaskExample> availableTasks = new List<TaskExample>();

        foreach (var task in _tasksService.Tasks)
        {
            if (task.Value.State == TaskState.Available)
            {
                availableTasks.Add(task.Value);
            }
        }

        for (int i = 0; i < _spawnedButtons.Count; i++)
        {
            if (i >= availableTasks.Count)
            {
                _spawnedButtons[i].gameObject.SetActive(false);
                continue;
            }
            _spawnedButtons[i].gameObject.SetActive(true);
            _spawnedButtons[i].SetTaskData(availableTasks[i]);
        }

    }

    private void UpdateTasksText()
    {
        string taskText = string.Empty;
        foreach (var task in _tasksService.Tasks)
        {
            taskText += $"ID - {task.Key} Day - {task.Value.Day} TimeLimit - {task.Value.TimeLimit:F1} State - {task.Value.State}\n";
        }
        _allTasksText.text = taskText;
    }




    private void Update()
    {
        if (!isActive) return;

        string tasksText = string.Empty;
        foreach (var task in _tasksService.Tasks)
        {
            tasksText += $"ID - {task.Key} Day - {task.Value.Day} TimeLimit - {task.Value.TimeLimit:F1} State - {task.Value.State}\n";
        }
        _allTasksText.text = tasksText;
    }


    public void CleanUp()
    {
        if (_inputService != null) _inputService.OnShowTasksMenu -= OpenClose;
        if (_tasksService != null) _tasksService.OnTasksChanged -= UpdateView;


        for (int i = _spawnedButtons.Count - 1; i >= 0; i--)
        {
            _spawnedButtons[i].OnClick -= HandleButton_OnClick;
        }
        _spawnedButtons.Clear();
    }

}
