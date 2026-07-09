using UnityEngine;
using VContainer;

public class InteractSpeaking : MonoBehaviour, IInteractable
{
    [Inject]
    public void Construct(DialogueService dialogueService)
    {
        _dialogueService = dialogueService;
    }

    private DialogueService _dialogueService;
    [SerializeField] private string nodeName;

    public void Interact(GameObject player)
    {
        _dialogueService.StartDialogue(nodeName);
    }
}
