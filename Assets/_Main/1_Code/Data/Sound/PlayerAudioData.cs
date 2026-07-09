using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAudioData", menuName = "Scriptable Objects/PlayerAudioData")]
public class PlayerAudioData : ScriptableObject
{
    [Header("Movement")]
    public AudioClip[] footstepClips;
    public AudioClip jumpClip;
    public AudioClip landClip;

    public AudioClip sprintClip;

    [Header("Actions")]
    [Tooltip("Звук при нажатии кнопки 'Использовать' (рычаг, кнопка)")]
    public AudioClip interactClip;
    [Tooltip("Звук получения урона при падении")]
    public AudioClip fallDamageClip;

    public AudioClip deadClip;

    public AudioClip flashLight;
}
