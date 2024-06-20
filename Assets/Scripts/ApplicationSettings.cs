using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationSettings : MonoBehaviour
{
    void Awake()
    {
        Application.runInBackground = true;
    }
}
