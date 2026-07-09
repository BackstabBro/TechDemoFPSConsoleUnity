using System;
using UnityEngine;
using VContainer;

public class PlayerService : IDisposable
{
    private readonly GameData _gameData;
    private readonly IGameParameter<float> _healthParam;
    private bool _isAlive = true;

    public event Action OnPlayerDead;
    public event Action<float> OnHealthChanged;

    [Inject]
    public PlayerService(GameData gameData)
    {
        _gameData = gameData;
        PlayerParameters playerData = _gameData.PlayerParameters;
        _healthParam = playerData.Health;

    }

    public void Init()
    {
        _healthParam.OnChanged += OnHealthParameterChanged;
        _healthParam.Set(100f);
    }


    private void OnHealthParameterChanged(IGameParameter param)
    {
        float currentHealth = _healthParam.Value;

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0 && _isAlive)
        {
            _isAlive = false;
            OnPlayerDead?.Invoke();
        }
        else if (currentHealth > 0 && !_isAlive)
        {
            _isAlive = true;
        }
    }

    public void TakeDamage(float amount)
    {
        if (!_isAlive) return;
        float currentHealth = _healthParam.Value;
        float newHealth = currentHealth - amount;
        _healthParam.Set(newHealth);

        Debug.Log($"Игрок получил урон: {amount}. Текущее здоровье: {_healthParam.Value}");
    }

    public float GetHealth()
    {
        return _healthParam.Value;
    }

    public void Dispose()
    {
        if (_healthParam != null)
        {
            _healthParam.OnChanged -= OnHealthParameterChanged;
        }
    }
}
