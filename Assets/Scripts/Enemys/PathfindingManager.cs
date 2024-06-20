using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;
    private Map _map;
    private Vector2[] _flyingPath;
    private float scale = 1.5f;
    private Vector2[] neighbors;
    private Vector2 offset = new Vector2(3.5f, 4f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            neighbors = new Vector2[] { new Vector2(scale, 0), new Vector2(-scale, 0), new Vector2(0, scale), new Vector2(0, -scale) };
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _map = Map.Instance;
        _flyingPath = FindFlyingPath(_map.startCell.transform.position, _map.goalCell.transform.position);
    }

    public bool IsPathAvailable()
    {
        return FindPath(_map.startCell.transform.position, _map.goalCell.transform.position).Length > 0;
    }

    public Vector2[] GetFlyingPath()
    {
        return _flyingPath;
    }

    public Vector2[] FindPath(Vector2 start, Vector2 target)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();

            if (current == target)
            {
                return ReconstructPath(cameFrom, start, target);
            }

            foreach (Vector2 dir in neighbors)
            {
                Vector2 neighbor = current + dir + offset;
                if (neighbor - offset == target || (_map.IsFreeField((int) (neighbor.x / scale), (int) (neighbor.y / scale)) && !cameFrom.ContainsKey(neighbor - offset)))
                {
                    neighbor -= offset;
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        Debug.Log($"No path found");
        return new Vector2[0];
    }

    public Vector2[] FindFlyingPath(Vector2 start, Vector2 target)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();

            if (current == target)
            {
                return ReconstructPath(cameFrom, start, target);
            }

            foreach (Vector2 dir in neighbors)
            {
                Vector2 neighbor = current + dir + offset;
                if (neighbor - offset == target || (neighbor.x < _map.GetSize() * scale && neighbor.x >= 0 && neighbor.y < _map.GetSize() * scale && neighbor.y >= 0 && !cameFrom.ContainsKey(neighbor - offset)))
                {
                    neighbor -= offset;
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        Debug.Log($"No flyingpath found");
        return new Vector2[0];
    }

    private Vector2[] ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 start, Vector2 target)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 current = target;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);
        path.Reverse();

        return path.ToArray();
    }
}
