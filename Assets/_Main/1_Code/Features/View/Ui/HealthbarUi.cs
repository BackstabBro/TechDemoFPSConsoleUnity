using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class HealthbarUi : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    private PlayerService _playerService;

    [Inject]
    public void Construct(PlayerService playerService)
    {
        _playerService = playerService;
    }

    public void Init()
    {
        _playerService.OnHealthChanged += UpdateHealthVisuals;
    }

    public void CleanUp()
    {
        _playerService.OnHealthChanged -= UpdateHealthVisuals;
    }

    public void Startup()
    {
        gameObject.SetActive(true);
        _healthSlider.value = _playerService.GetHealth() / 100f;
    }

    private void UpdateHealthVisuals(float health)
    {
        float value = health / 100f;

        _healthSlider.value = value;
    }
}
