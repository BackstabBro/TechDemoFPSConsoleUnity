using System;
using TMPro;
using UnityEngine;
using VContainer;

public class Ui_GameTime : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeTMPText;
    private GameData _gameData;

    [Inject]
    public void Construct(GameData gameData)
    {
        _gameData = gameData;
    }

    public void Init()
    {
        _gameData.TimeParameters.CurrentTime.OnChanged += CurrentTime_OnChanged;
        UpdateText(_gameData.TimeParameters.CurrentTime);
    }

    public void Startup()
    {
        gameObject.SetActive(true);
    }

    private void CurrentTime_OnChanged(IGameParameter currentTime)
    {
        UpdateText(currentTime);
    }

    private void UpdateText(IGameParameter currentTimeParameter)
    {
        if (currentTimeParameter is IGameParameter<TimeSpan> timeParam)
        {
            string dayName = _gameData.TimeParameters.CurrentLevelName.Value;
            string timeString = timeParam.Value.ToString(@"hh\:mm");
            _timeTMPText.text = $"{dayName} - {timeString}";
        }
    }

    public void CleanUp()
    {
        if (_gameData != null)
        {
            _gameData.TimeParameters.CurrentTime.OnChanged -= CurrentTime_OnChanged;
        }
    }

    private void OnDestroy()
    {
       
    }
}
