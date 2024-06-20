using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip music;

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
        DontDestroyOnLoad(this);
        audioSource.clip = music;
        audioSource.Play();
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
}
