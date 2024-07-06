using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundeffectManager : MonoBehaviour, ISubscriber<PlaySoundSignal>
{
    public static SoundeffectManager instance;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _shockwave;
    [SerializeField] AudioClip _looseLife;
    [SerializeField] AudioClip _newRound;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        Signalbus.Subscirbe<PlaySoundSignal>(this);
        DontDestroyOnLoad(this);
    }

    public void SetVolume(float volume)
    {
        if (volume < 0f || volume > 1f) return;
        _audioSource.volume = volume;
    }

    public float GetVolume()
    {
        return _audioSource.volume;
    }

    public void OnSignalReceived(PlaySoundSignal s)
    {
        Debug.Log($"Play Clip {s.clipName}");
        switch (s.clipName)
        {
            case "shockwave":
                _audioSource.clip = _shockwave;
                _audioSource.Play();
                break;
            case "looseLife":
                _audioSource.clip = _looseLife;
                _audioSource.Play();
                break;
            case "newRound":
                _audioSource.clip = _newRound;
                _audioSource.time = 1.5f;
                _audioSource.Play();
                break;
        }
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe(this);
    }
}
