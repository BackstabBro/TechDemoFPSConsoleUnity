using UnityEngine;

public class TestInteract : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        Destroy(gameObject);
    }

}
