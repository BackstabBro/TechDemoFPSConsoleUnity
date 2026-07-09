using UnityEngine;
using VContainer;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] private AudioSource _stepsSource;
    [SerializeField] private AudioSource _actionsSource;
    private PlayerAudioData _audioData;

    [Inject]
    public void Construct(PlayerAudioData playerAudioData)
    {
        _audioData = playerAudioData;
    }


    public void PlayFootStepsSound(bool isMovingOnTheGround, bool isSprinting)
    {
        if (isMovingOnTheGround)
        {
            _stepsSource.pitch = isSprinting ? 2f : 1.0f;

            if (!_stepsSource.isPlaying)
            {
                // Берём самый первый клип из массива шагов в SO
                if (_audioData.footstepClips != null && _audioData.footstepClips.Length > 0)
                {
                    _stepsSource.clip = _audioData.footstepClips[0];
                    _stepsSource.Play();
                }
            }
        }
        else
        {
            if (_stepsSource.isPlaying)
            {
                _stepsSource.Stop();
            }
        }
    }


    public void PlayJump()
    {
        if (_audioData.jumpClip != null)
            _actionsSource.PlayOneShot(_audioData.jumpClip);
    }

    public void PlayLand()
    {
        if (_audioData.landClip != null)
            _actionsSource.PlayOneShot(_audioData.landClip);
    }

    public void PlayFallDamage()
    {
        if (_audioData.fallDamageClip != null)
            _actionsSource.PlayOneShot(_audioData.fallDamageClip);
    }

    public void PlayInteract()
    {
        if (_audioData.interactClip != null)
            _actionsSource.PlayOneShot(_audioData.interactClip);
    }

    public void PlaySprint()
    {
        if (_audioData.interactClip != null)
            _actionsSource.PlayOneShot(_audioData.sprintClip);
    }

    public void PlayFlashlight()
    {
        if (_audioData.interactClip != null)
            _actionsSource.PlayOneShot(_audioData.flashLight);
    }

    public void PlayDead()
    {
        if (_audioData.interactClip != null)
            _actionsSource.PlayOneShot(_audioData.deadClip);
    }
}
