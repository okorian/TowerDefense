using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, 
    ISubscriber<NewTowerSignal>, ISubscriber<SpawnEnemysSignal>, ISubscriber<SpawnEnemyWaveSignal>, ISubscriber<GameLostSignal>, ISubscriber<TowerSoldSignal>
{
    [SerializeField] EnemyData _defaultEnemy;
    [SerializeField] EnemyWaveManager _enemyWaveManager;
    Vector2 _spawnPosition;
    Vector2[] _path;
    PathfindingManager _pathfindingManager;
    Map _map;
    Coroutine _spawnEnemyWave;

    void Start()
    {
        _pathfindingManager = PathfindingManager.Instance;
        _map = Map.Instance;
        Signalbus.Subscirbe<NewTowerSignal>(this);
        Signalbus.Subscirbe<SpawnEnemysSignal>(this);
        Signalbus.Subscirbe<SpawnEnemyWaveSignal>(this);
        Signalbus.Subscirbe<GameLostSignal>(this);
        Signalbus.Subscirbe<TowerSoldSignal>(this);
        _spawnPosition = _map.startCell.transform.position;
        _path = _pathfindingManager.FindPath(_map.startCell.transform.position, _map.goalCell.transform.position);
    }

    void SpawnEnemy(EnemyData enemyData, Vector2 spawnPosition, Vector2[] path) //, int liveMult, int armorBuff, int speedBuff)
    {
        GameObject enemy = Instantiate(enemyData.enemyPrefab, new Vector3(spawnPosition.x, spawnPosition.y, -1), Quaternion.identity);
        enemy.transform.parent = transform;
        enemy.GetComponent<Enemy>().Initialize(enemyData, path);//, liveMult, armorBuff, speedBuff);
        Signalbus.Fire<EnemySpawnedSignal>(new EnemySpawnedSignal());
    }

    public void OnSignalReceived(NewTowerSignal signal)
    {
        if (_path.Contains(signal.position))
        {
            _path = _pathfindingManager.FindPath(_map.startCell.transform.position, _map.goalCell.transform.position);
        }
    }

    public void OnSignalReceived(SpawnEnemysSignal signal)
    {
        StartCoroutine(SpawnSplittedEnemys(_defaultEnemy, signal.position, signal.path, signal.count));//, signal.liveMult, signal.armorBuff, signal.speedBuff));
    }

    public void OnSignalReceived(SpawnEnemyWaveSignal signal)
    {
        _spawnEnemyWave = StartCoroutine(SpawnEnemyWave(_enemyWaveManager.GenerateWave(signal.round)));
    }

    public void OnSignalReceived(GameLostSignal signal)
    {
        if(_spawnEnemyWave != null)
        {
            StopCoroutine(_spawnEnemyWave);
            _spawnEnemyWave = null;
        }
    }

    IEnumerator SpawnEnemyWave(EnemyWave wave)
    {
        while (wave.enemyQueue.Count > 0)
        {
            EnemyData enemy = wave.enemyQueue.Dequeue();
            if (enemy.isFlying)
            {
                SpawnEnemy(enemy, _spawnPosition, _pathfindingManager.GetFlyingPath());
            }
            else
            {
                SpawnEnemy(enemy, _spawnPosition, _path);
            }
            yield return new WaitForSeconds(wave.spawnSpeed);
        }
    }

    IEnumerator SpawnSplittedEnemys(EnemyData enemy, Vector3 position, Vector2[] path, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(enemy, position, path);
            
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<NewTowerSignal>(this);
        Signalbus.Unsubscribe<SpawnEnemysSignal>(this);
        Signalbus.Unsubscribe<SpawnEnemyWaveSignal>(this);
        Signalbus.Unsubscribe<GameLostSignal>(this);
        Signalbus.Unsubscribe<TowerSoldSignal>(this);
    }

    public void ResetPath()
    {
        _path = _pathfindingManager.FindPath(_map.startCell.transform.position, _map.goalCell.transform.position);
    }

    public void OnSignalReceived(TowerSoldSignal signal)
    {
        ResetPath();
    }
}
