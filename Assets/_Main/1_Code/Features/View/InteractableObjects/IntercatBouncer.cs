using UnityEngine;

public class IntercatBouncer : MonoBehaviour, IInteractable
{
    [SerializeField] private float bounceForce = 10f;

    public void Interact(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    }



}
