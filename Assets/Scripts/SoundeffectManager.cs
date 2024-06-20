using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundeffectManager : MonoBehaviour, ISubscriber<PlaySoundSignal>
{
    public static SoundeffectManager instance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip shockwave;
    [SerializeField] AudioClip ui;

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
        audioSource.volume = volume;
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }

    public void OnSignalReceived(PlaySoundSignal s)
    {
        switch (s.clipName)
        {
            case "ui":
                audioSource.clip = ui;
                audioSource.Play();
                break;
            case "shockwave":
                audioSource.clip = shockwave;
                audioSource.Play();
                break;
        }
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe(this);
    }
}
