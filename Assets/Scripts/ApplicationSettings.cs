using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationSettings : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotspot = new Vector2(10, 10);

    void Awake()
    {
        Application.runInBackground = true;
        ChangeCursor(cursorTexture, hotspot);
    }

    public void ChangeCursor(Texture2D texture, Vector2 hotspot)
    {
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }
}
