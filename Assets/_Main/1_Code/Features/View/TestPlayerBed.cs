using UnityEngine;
using VContainer;

public class TestPlayerBed : MonoBehaviour, IInteractable
{
    private DayTimeService _gameDayTimeService;
    private TaskService _tasksService;

    [Inject]
    public void Construct(DayTimeService gameDayTimeService, TaskService tasksService)
    {
        _gameDayTimeService = gameDayTimeService;
        _tasksService = tasksService;
    }

    public void Interact(GameObject player)
    {
       _tasksService.FailAllActiveTasksOnDayEnd();
       _gameDayTimeService.GoToNextDay();
    }

}
