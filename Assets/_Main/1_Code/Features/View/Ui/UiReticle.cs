using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UiReticle : MonoBehaviour
{
    [Inject]
    public void Construct(PlayerInteractController playerInteractController)
    {
        _playerInteractController = playerInteractController;
        _playerInteractController.OnInteractableSelected += HandleReticleColor;
    }

    [SerializeField] Color regularColor;
    [SerializeField] Color interactColor;
    [SerializeField] Image reticleImage;
    private PlayerInteractController _playerInteractController;

    private void HandleReticleColor(bool selected)
    {
        if (selected)
        {
            reticleImage.color = interactColor;
        }
        else
        {
            reticleImage.color = regularColor;
        }
    }

    private void OnDestroy()
    {
        _playerInteractController.OnInteractableSelected -= HandleReticleColor;
    }

}
