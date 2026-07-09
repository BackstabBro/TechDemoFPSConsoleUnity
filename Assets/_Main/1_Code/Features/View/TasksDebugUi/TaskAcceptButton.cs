using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TaskAcceptButton : MonoBehaviour
{
    [SerializeField] private Button _acceptButton;
    [SerializeField] private TMP_Text _taskText;
    private TaskExample _taskData;

    public event Action<TaskExample> OnClick;

    public void Init()
    {
        _acceptButton.onClick.AddListener(Click);
    }

    public void SetTaskData(TaskExample taskData)
    {
        _taskData = taskData;
        _taskText.text = $"{taskData.ID}";
    }

    private void Click()
    {
        OnClick?.Invoke(_taskData);
    }


    public void OnDisable()
    {

    }
}
