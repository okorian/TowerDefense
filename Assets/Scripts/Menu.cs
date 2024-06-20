using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    List<Resolution> resolutions = new List<Resolution>
    {
        new Resolution { width = 3840, height = 2160 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1680, height = 1050 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1536, height = 864 },
        new Resolution { width = 1440, height = 900 },
        new Resolution { width = 1366, height = 768 },
        new Resolution { width = 1280, height = 800 },
        new Resolution { width = 1280, height = 720 }
    };

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        resolutionDropdown.onValueChanged.AddListener(delegate { ChangeResolution(); });
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void ChangeResolution()
    {
        int index = resolutionDropdown.value;
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        if (Screen.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void SetMusicVolume(float volume)
    {
        MusicManager.instance.SetVolume(volume);
    }

    public void SetEffectVolume(float volume)
    {
        SoundeffectManager.instance.SetVolume(volume);
    }
}
