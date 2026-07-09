using System;
using UnityEngine;
using VContainer;
using Yarn.Unity;

public class DialogueService : IDisposable
{
    private readonly GameStateService _gameStateService;
    private readonly DialogueRunner _dialogueRunner;
    private readonly TaskService _tasksService;

    [Inject]
    public DialogueService(
        GameStateService gameStateService,
        DialogueRunner dialogueRunner,
        TaskService tasksService)
    {
        _gameStateService = gameStateService;
        _dialogueRunner = dialogueRunner;
        _tasksService = tasksService;
    }

    public void Init()
    {
        _dialogueRunner.onDialogueComplete.AddListener(CloseDialogue);
        _dialogueRunner.AddCommandHandler<string>("addTask", AddTask);
        _dialogueRunner.AddCommandHandler<string>("completeTask", CompleteTask);
        _dialogueRunner.AddCommandHandler("quit", QuitDialogue);
        _dialogueRunner.AddFunction<string, bool>("hasActiveTask", HasActiveTask);
        _dialogueRunner.AddFunction<string, bool>("isTaskAvaible", IsTaskAvaible);
    }


    public void Dispose()
    {
        _dialogueRunner.onDialogueComplete.RemoveListener(CloseDialogue);
        _dialogueRunner.RemoveCommandHandler("addTask");
        _dialogueRunner.RemoveCommandHandler("completeTask");
        _dialogueRunner.RemoveCommandHandler("quit");
        _dialogueRunner.RemoveFunction("hasActiveTask");
        _dialogueRunner.RemoveFunction("isTaskAvaible");
    }


    public void StartDialogue(string nodeName)
    {
        if (_dialogueRunner.IsDialogueRunning)
        {
            Debug.LogWarning($"[{GetType().Name}] Попытка запустить диалог '{nodeName}', но другой диалог уже идет!");
            return;
        }

        if (!_dialogueRunner.Dialogue.NodeExists(nodeName))
        {
            Debug.LogWarning($"[{GetType().Name}] Попытка запустить диалог '{nodeName}', которого нет в проекте!");
            return;
        }

        _gameStateService.PushState(GameStates.Dialogue);
        _dialogueRunner.StartDialogue(nodeName);
    }

    private void CloseDialogue()
    {
        _gameStateService.PopState();
    }


    private void AddTask(string taskId)
    {
        _tasksService.AcceptTask(taskId);
    }

    private void CompleteTask(string taskId)
    {
        _tasksService.CompleteTask(taskId);
    }


    private void QuitDialogue()
    {
        _dialogueRunner.Stop();
    }


    private bool HasActiveTask(string taskId)
    {
        return _tasksService.HasActiveTask(taskId);
    }

    private bool IsTaskAvaible(string taskId)
    {
        return _tasksService.IsTaskAvailable(taskId);
    }
}
