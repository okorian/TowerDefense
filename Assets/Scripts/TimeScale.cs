using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour
{
    [SerializeField] Image pause, play, fast, faster;

    public void ScaleTime(float speed)
    {
        Time.timeScale = speed;

        switch(speed)
        {
            case 0f:
                pause.color = Color.white;
                play.color = Color.black;
                fast.color = Color.black;
                faster.color = Color.black;
                break;
            case 1f:
                pause.color = Color.black;
                play.color = Color.white;
                fast.color = Color.black;
                faster.color = Color.black;
                break;
            case 2f:
                pause.color = Color.black;
                play.color = Color.black;
                fast.color = Color.white;
                faster.color = Color.black;
                break;
            case 4f:
                pause.color = Color.black;
                play.color = Color.black;
                fast.color = Color.black;
                faster.color = Color.white;
                break;
        }
    }
}
