using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public Map map;

    void Start()
    {
        Camera cam = Camera.main;
        float requiredSize = map.GetSize() / 2 + 5;
        cam.orthographicSize = requiredSize;
        cam.transform.position = new Vector3(map.GetSize() / 2, map.GetSize() / 2, -10);
    }
}
