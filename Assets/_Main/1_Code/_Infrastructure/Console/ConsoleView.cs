using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ConsoleView : MonoBehaviour
{
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text logText;
    [SerializeField] private ScrollRect logScrollRect;

    private ConsoleCommandService _commandService;
    private InputService _inputService;
    private GameStateService _gameStateService;
    private bool isActive = false;

    private readonly List<string> _logLines = new List<string>();
    private const int MaxLines = 1000; // Лимит строк

    [Inject]
    public void Construct(ConsoleCommandService commandService, InputService inputService, GameStateService gameStateService)
    {
        _commandService = commandService;
        _inputService = inputService;
        _gameStateService = gameStateService;

    }

    public void Init()
    {
        _inputService.OnOpenConsole += Show;
        inputField.onSubmit.AddListener(OnSubmitCommand);
        _commandService.OnLogReceived += AppendLogLine;
        _commandService.OnConsoleCleared += ClearUI;
    }

    public void CleanUp()
    {
        if (_inputService != null) _inputService.OnOpenConsole -= Show;
        if (_commandService != null)
        {
            _commandService.OnLogReceived -= AppendLogLine;
            _commandService.OnConsoleCleared -= ClearUI;
        }
        inputField.onSubmit.RemoveListener(OnSubmitCommand);
    }

    public void Startup()
    {
        consolePanel.SetActive(false);
    }

    private void Show()
    {
        isActive = !isActive;
        consolePanel.SetActive(isActive);

        if (isActive)
        {
            inputField.ActivateInputField(); // Сразу фокусим ввод
            _gameStateService.PushState(GameStates.Console);
            return;
        }
        else
        {
            inputField.DeactivateInputField();
            inputField.text = string.Empty;
            _gameStateService.PopState();
        }
    }


    public void Hide()
    {
        isActive = false;
        consolePanel.SetActive(isActive);
        inputField.DeactivateInputField();
        inputField.text = string.Empty;
    }


    private void OnSubmitCommand(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        if (text.Trim() == "`")
        {
            Show();
            return;
        }

        _commandService.RunCommand(text);
        inputField.text = string.Empty;
        if (isActive) inputField.ActivateInputField();
    }

    private void AppendLogLine(string line)
    {
        _logLines.Add(line);
        if (_logLines.Count > MaxLines) _logLines.RemoveAt(0);

        UpdateLogText();
        ScrollToBottom(); // Скроллим вниз при каждом новом сообщении
    }

    private void ClearUI()
    {
        _logLines.Clear();
        logText.text = string.Empty;
        ScrollToBottom();
    }

    private void UpdateLogText()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var log in _logLines) sb.AppendLine(log);
        logText.text = sb.ToString();
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        logScrollRect.verticalNormalizedPosition = 0f;
    }

    private void OnDestroy()
    {

    }
}
