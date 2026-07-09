using System;
using UnityEngine;
using VContainer;

public class PlayerInteractController : MonoBehaviour
{
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask LayerMask;

    public event Action<bool> OnInteractableSelected;
    public event Action OnInteractTry;

    private Transform _origin;
    private IInteractable _currentInteractable;
    private bool _wasInteractableSelected;

    [Inject]
    public void Construct()
    {

    }


    public void Interact()
    {
        OnInteractTry?.Invoke();
        _currentInteractable?.Interact(gameObject);
    }


    public void RayCastInteract(Transform origin)
    {
        _origin = origin;
        IInteractable foundInteractable = null;

        if (Physics.Raycast(_origin.position, _origin.forward, out RaycastHit hit, interactDistance, LayerMask))
        {
            foundInteractable = hit.collider.GetComponent<IInteractable>();
        }

        _currentInteractable = foundInteractable;

        bool isCurrentlyInteractable = _currentInteractable != null; //Проверяем, изменился ли объект прицела по сравнению с прошлым кадром

        if (isCurrentlyInteractable != _wasInteractableSelected)
        {
            _wasInteractableSelected = isCurrentlyInteractable;
            OnInteractableSelected?.Invoke(isCurrentlyInteractable);
        }
    }

}
