using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour, ISubscriber<GameLostSignal>, ISubscriber<RestartGameSignal>
{
    [SerializeField] Camera _mainCamera;
    [SerializeField] DisplaySelection _displaySelection;
    [SerializeField] GameObject towerPreviewPrefab;
    [SerializeField] GameObject _menu;
    [SerializeField] GameObject _feedbackPanel;
    [SerializeField] TimeScale _timeScale;
    [SerializeField] ProjectileFactory _arrowFactory;
    [SerializeField] ProjectileFactory _canonFactory;
    [SerializeField] ProjectileFactory _fireFactory;
    [SerializeField] TowerData _tdArrow;
    [SerializeField] TowerData _tdCannon;
    [SerializeField] TowerData _tdFire;
    [SerializeField] TowerData _tdIce;
    [SerializeField] TowerData _tdGold;
    [SerializeField] Transform _canvasTransform;

    GameObject towerPreviewInstance;
    Map _map;
    TowerData _towerData;
    LayerMask _enemyLayer;
    bool _hasLost;

    private void Start()
    {
        Signalbus.Subscirbe<GameLostSignal>(this);
        Signalbus.Subscirbe<RestartGameSignal>(this);
        _map = Map.Instance;
        _enemyLayer = LayerMask.GetMask("Enemy");
        _hasLost = true;
    }

    void Update()
    {
        // menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_menu.activeSelf)
            {
                _timeScale.ScaleTime(1f);
                _menu.SetActive(false);
            }
            else
            {
                _timeScale.ScaleTime(0f);
                _menu.SetActive(true);
            }
        }

        if (!_hasLost)
        {
            // Hotkeys timescale
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _timeScale.ScaleTime(0f);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _timeScale.ScaleTime(1f);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                _timeScale.ScaleTime(2f);
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                _timeScale.ScaleTime(4f);
            }

            // Hotkeys select tower
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectTower(_tdArrow);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectTower(_tdCannon);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectTower(_tdFire);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectTower(_tdIce);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectTower(_tdGold);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        Transform hitTransform = hit.transform;
                        if (hitTransform.CompareTag("Cell"))
                        {
                            _displaySelection.Select(null);

                            if (_towerData != null)
                            {
                                Cell cell = hitTransform.GetComponent<Cell>();

                                if (PlaceTowerAtMousePosition(cell) && !Input.GetKey(KeyCode.LeftShift))
                                {
                                    _towerData = null;
                                    Destroy(towerPreviewInstance);
                                }
                            }
                        }
                        else if (hitTransform.CompareTag("Tower") || hitTransform.CompareTag("Enemy"))
                        {
                            if (towerPreviewInstance != null)
                            {
                                _towerData = null;
                                Destroy(towerPreviewInstance);
                            }
                            _displaySelection.Select(hitTransform.gameObject);
                        }
                        else
                        {
                            if (towerPreviewInstance != null)
                            {
                                _towerData = null;
                                Destroy(towerPreviewInstance);
                            }
                            _displaySelection.Select(null);
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (towerPreviewInstance != null)
                {
                    _towerData = null;
                    Destroy(towerPreviewInstance);
                }
            }
        }

        if (_towerData != null && towerPreviewInstance != null)
        {
            UpdateTowerPreview();
        }
    }

    bool PlaceTowerAtMousePosition(Cell cell)
    {
        switch (_towerData.name)
        {
            case "ArrowTower":
                _towerData.projectileFactory = _arrowFactory;
                break;
            case "CanonTower":
                _towerData.projectileFactory = _canonFactory;
                Debug.Log("Factory: " + _towerData.projectileFactory);
                break;
            case "FireTower":
                _towerData.projectileFactory = _fireFactory;
                break;
        }
        return !IsEnemyOnCell(cell) && CanPlaceTower(cell); //_map.PlaceTower(cell.x, cell.y, _towerData);
    }

    public bool IsEnemyOnCell(Cell cell)
    {
        MeshCollider cellCollider = cell.GetComponent<MeshCollider>();
        Vector2 point = cellCollider.bounds.center;
        Vector2 size = cellCollider.bounds.size;

        Collider2D hitCollider = Physics2D.OverlapBox(point, size, 0, _enemyLayer);
        return hitCollider != null;
    }

    public bool CanPlaceTower(Cell cell)
    {
        bool result = _map.PlaceTower(cell.x, cell.y, _towerData);
        if (!result)
        {
            StartCoroutine(ShowFeedbackPanel());
        }
        return result;
    }

    private IEnumerator ShowFeedbackPanel()
    {
        GameObject feedbackPanel = Instantiate(_feedbackPanel);
        feedbackPanel.transform.parent = _canvasTransform;
        Vector2 position = Input.mousePosition;
        position.y += 15f;
        feedbackPanel.transform.position = position;
        feedbackPanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Destroy(feedbackPanel);
    }

    public void SelectTower(TowerData data)
    {
        _towerData = data;

        if (towerPreviewInstance != null)
        {
            Destroy(towerPreviewInstance);
        }

        towerPreviewInstance = Instantiate(towerPreviewPrefab);
        towerPreviewInstance.GetComponent<TowerPreview>().Initialize(data);
        towerPreviewInstance.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        towerPreviewInstance.SetActive(false);
    }

    void UpdateTowerPreview()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Transform hitTransform = hit.transform;
            if (hitTransform.CompareTag("Cell"))
            {
                Cell cell = hitTransform.GetComponent<Cell>();
                if (cell != null)
                {
                    towerPreviewInstance.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, -3);
                    towerPreviewInstance.SetActive(true);
                }
            }
            else
            {
                towerPreviewInstance.SetActive(false);
            }
        }
        else
        {
            towerPreviewInstance.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<GameLostSignal>(this);
        Signalbus.Unsubscribe<RestartGameSignal>(this);
    }

    public void OnSignalReceived(GameLostSignal signal)
    {
        _hasLost = true;

        if (towerPreviewInstance != null)
        {
            Destroy(towerPreviewInstance);
        }
    }

    public void OnSignalReceived(RestartGameSignal signal)
    {
        _hasLost = false;
    }

    public void StartGame()
    {
        _hasLost = false;
    }
}
