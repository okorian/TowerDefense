using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    float _range;
    [SerializeField] LineRenderer _lineRenderer;

    public void Initialize(TowerData data)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = data.towerSprite;
        _range = data.range[0];

        _lineRenderer.useWorldSpace = false;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 51;

        int segments = 50;
        float angle = 360f / segments;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angle);
            float x = Mathf.Cos(rad) * _range;
            float y = Mathf.Sin(rad) * _range;
            points[i] = new Vector3(x, y, 0f);
        }

        _lineRenderer.SetPositions(points);
    }
}
