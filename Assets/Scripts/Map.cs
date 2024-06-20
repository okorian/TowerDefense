using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour, ISubscriber<RestartGameSignal>
{
    public static Map Instance;
    int _size = 15;
    bool[][] _hasTowers;
    GameObject[][] _cells;
    [SerializeField] TowerData _towerData;
    [SerializeField] GameObject _grid;
    [SerializeField] GameObject _towerParent;
    [SerializeField] EnemySpawner _enemySpawner;
    [SerializeField] Material _ground;
    public GameObject startCell, goalCell;
    GameController _gameController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _hasTowers = new bool[_size][];
        for (int i = 0; i < _hasTowers.Length; i++)
        {
            _hasTowers[i] = new bool[_size];
        }

        _cells = new GameObject[_size][];
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i] = new GameObject[_size];
        }

        GenerateGrid();
    }

    private void Start()
    {
        _gameController = GameController.Instance;
        Signalbus.Subscirbe<RestartGameSignal>(this);
    }

    // Place Tower if field is free
    // return true when placed, false when field was occupied
    public bool PlaceTower(int x, int y, TowerData data)
    {
        if(IsFreeField(x, y))
        {
            GameObject tower = Instantiate(data.towerPrefab, _cells[x][y].gameObject.transform.position + new Vector3(0, 0, -1), new Quaternion());
            tower.GetComponent<Tower>().Initialize(x, y, data);
            tower.transform.parent = _towerParent.transform;
            _hasTowers[x][y] = true;
            if (PathfindingManager.Instance.IsPathAvailable() && _gameController.PayGold(data.price[0]))
            {
                Signalbus.Fire<NewTowerSignal>(new NewTowerSignal() { position = new Vector2(x * 1.5f - 3.5f, y * 1.5f - 4f) });
                return true;
            }
            else
            {
                // Wenn kein Pfad, oder nicht genug Gold verfügbar ist, Rückgängig machen der Platzierung
                Destroy(tower);
                _hasTowers[x][y] = false;
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.name = "cell";
                cell.transform.position = new Vector3(x, y, 2);
                cell.transform.parent = _grid.transform;
                cell.tag = "Cell";
                Destroy(cell.GetComponent<MeshRenderer>());
                Cell c = cell.AddComponent<Cell>();
                c.x = x;
                c.y = y;
                _cells[x][y] = cell;
            }
        }

        startCell = GameObject.CreatePrimitive(PrimitiveType.Quad);
        startCell.name = "startCell";
        startCell.transform.position = new Vector2(-1, _size / 2);
        startCell.transform.parent = _grid.transform;
        Destroy(startCell.GetComponent<MeshRenderer>());
        //startCell.GetComponent<MeshRenderer>().material.color = Color.green;

        goalCell = GameObject.CreatePrimitive(PrimitiveType.Quad);
        goalCell.name = "goalCell";
        goalCell.transform.position = new Vector2(_size, _size / 2);
        goalCell.transform.parent = _grid.transform;
        Destroy(goalCell.GetComponent<MeshRenderer>());
        //goalCell.GetComponent<MeshRenderer>().material.color = Color.yellow;

        _grid.transform.position = new Vector2(0, 0);

        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        gameObject.transform.position = new Vector3(-3.5f, -4, 0);
    }

    public bool IsFreeField(int x, int y)
    {
        return x < _size && y < _size && x >= 0 && y >= 0 && !_hasTowers[x][y];
    }

    public void RemoveTower(int x, int y)
    {
        if(x < _size && x >= 0 && y < _size && y >= 0)
        {
            _hasTowers[x][y] = false;
        }
    }

    public int GetSize()
    {
        return _size;
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe(this);
    }

    public void OnSignalReceived(RestartGameSignal signal)
    {
        _hasTowers = new bool[_size][];
        for (int i = 0; i < _hasTowers.Length; i++)
        {
            _hasTowers[i] = new bool[_size];
        }

        _enemySpawner.ResetPath();
    }
}
