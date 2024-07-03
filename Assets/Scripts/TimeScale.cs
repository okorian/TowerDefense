using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour, ISubscriber<RestartGameSignal>, ISubscriber<GameLostSignal>
{
    [SerializeField] Image pause, play, fast, faster;
    [SerializeField] TextMeshProUGUI _timeText;

    float _elapsedTime;
    bool _isRunning;

    private void Start()
    {
        Signalbus.Subscirbe<RestartGameSignal>(this);
        Signalbus.Subscirbe<GameLostSignal>(this);
    }

    private void Update()
    {
        if (_isRunning)
        {
            _elapsedTime += Time.unscaledDeltaTime;
            UpdateTimeDisplay();
        }
    }

    private void UpdateTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(_elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(_elapsedTime - minutes * 60);
        _timeText.text = "Playtime: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void PauseTime()
    {
        _isRunning = false;
    }

    public void ResumeTime()
    {
        _isRunning = true;
    }

    public void ResetTime()
    {
        _elapsedTime = 0f;
        UpdateTimeDisplay();
    }

    public void ScaleTime(float speed)
    {
        Time.timeScale = speed;

        switch(speed)
        {
            case 0f:
                PauseTime();

                pause.color = Color.white;
                play.color = Color.black;
                fast.color = Color.black;
                faster.color = Color.black;
                break;
            case 1f:
                ResumeTime();

                pause.color = Color.black;
                play.color = Color.white;
                fast.color = Color.black;
                faster.color = Color.black;
                break;
            case 2f:
                ResumeTime();

                pause.color = Color.black;
                play.color = Color.black;
                fast.color = Color.white;
                faster.color = Color.black;
                break;
            case 4f:
                ResumeTime();

                pause.color = Color.black;
                play.color = Color.black;
                fast.color = Color.black;
                faster.color = Color.white;
                break;
        }
    }

    public void OnSignalReceived(RestartGameSignal signal)
    {
        ResetTime();
        PauseTime();
    }

    public void OnSignalReceived(GameLostSignal signal)
    {
        PauseTime();
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<RestartGameSignal>(this);
        Signalbus.Unsubscribe<GameLostSignal>(this);
    }

    public float GetPlayTime()
    {
        return _elapsedTime;
    }
}
