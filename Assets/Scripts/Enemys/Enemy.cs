using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour, ISubscriber<NewTowerSignal>, ISubscriber<GameLostSignal>, ISubscriber<TowerSoldSignal>
{
    static int _killedGoldEnemies = 0;
    int _lives;
    int _maxLives;
    int _armor;
    int _bounty;
    float _speed;
    bool _isFlying;
    bool _isSplitting;
    bool _isScaling;
    bool _isGold;
    bool _isPushed;
    string _enemyName;
    Vector2[] _path;
    Vector2 _nextPos;
    Map _map;
    PathfindingManager _pathfindingManager;
    GameController _gameController;
    Coroutine _followPath;

    float _slow = 1f;
    HashSet<(IceTower, float)> _slowedBy = new HashSet<(IceTower, float)>();

    int _burnDmg;
    int _burnStacks;
    float _burnTimer;

    private void Update()
    {
        if (_isPushed && _followPath != null)
        {
            StopCoroutine(_followPath);
            _followPath = null;
        }

        if (_burnStacks > 0)
        {
            _burnTimer += Time.deltaTime;

            if (_burnTimer >= 1f)
            {
                _lives -= _burnDmg;
                if (_lives <= 0)
                {
                    if (_isGold)
                    {
                        _killedGoldEnemies++;
                    }
                    Signalbus.Fire<EnemyDiedSignal>(new EnemyDiedSignal() { value = _bounty });

                    if (_isSplitting)
                    {
                        Vector2[] splitPath = RecalculatePath();
                        Signalbus.Fire<SpawnEnemysSignal>(new SpawnEnemysSignal() { position = transform.position, path = splitPath, count = _gameController.GetRound() / 2 });
                    }

                    Destroy(gameObject);
                }
                _burnStacks--;
                _burnTimer = 0f;
            }
        }
    }

    public void Initialize(EnemyData data, Vector2[] path)//, int liveMult, int armorBuff, int speedBuff)
    {
        Signalbus.Subscirbe<NewTowerSignal>(this);
        Signalbus.Subscirbe<GameLostSignal>(this);
        Signalbus.Subscirbe<TowerSoldSignal>(this);
        this._map = Map.Instance;
        _pathfindingManager = PathfindingManager.Instance;
        _gameController = GameController.Instance;

        if (data.isScaling)
        {
            _lives = _gameController.GetRound() * 4;// * liveMult;
            _armor = Mathf.Min(_gameController.GetRound() / 5, 24);// + armorBuff;
        }
        else
        {
            _lives = data.lives;// * liveMult;
            _armor = data.armor;//  + armorBuff;
        }

        _maxLives = _lives;
        _speed = data.speed;// + speedBuff;
        _bounty = data.value;
        _isFlying = data.isFlying;
        _isSplitting = data.isSplitting;
        _isScaling = data.isScaling;
        _isGold = data.isGold;
        _enemyName = data.enemyName;
        _isPushed = false;

        if (_isGold)
        {
            _lives += _lives * _killedGoldEnemies;
            _bounty += _bounty * _killedGoldEnemies;
        }

        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        MoveOnPath(path);
    }

    public void TakeDamage(int dmg)
    {
        _lives -= Mathf.Max(dmg - _armor, 0);
        if(_lives <= 0)
        {
            Signalbus.Fire<EnemyDiedSignal>(new EnemyDiedSignal() { value = _bounty });

            if (_isSplitting)
            {
                Vector2[] splitPath = RecalculatePath();
                Signalbus.Fire<SpawnEnemysSignal>(new SpawnEnemysSignal() { position = transform.position, path = splitPath, count = _gameController.GetRound() / 2 });
            }

            Destroy(gameObject);
        }
    }

    public void MoveOnPath(Vector2[] path)
    {
        _path = path;
        if(_followPath != null)
        {
            StopCoroutine(_followPath);
        }
        _followPath = StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        int pathIndex = 0;
        while (pathIndex < _path.Length)
        {
            _nextPos = _path[pathIndex];
            while ((Vector2)transform.position != _nextPos)
            {
                transform.position = Vector2.MoveTowards(transform.position, _nextPos, _speed * _slow * Time.deltaTime);
                yield return null;
            }
            pathIndex++;
        }
        if (transform.position == _map.goalCell.transform.position)
        {
            if (!_isGold)
            {
                Signalbus.Fire<EnemyReachedGoalSignal>(new EnemyReachedGoalSignal());
            }
            Signalbus.Fire<EnemyDiedSignal>(new EnemyDiedSignal() { value = 0 });
            Destroy(gameObject);
        }
    }

    public Vector2[] RecalculatePath()
    {
        if (_isFlying)
        {
            return _path;
        }

        if(_nextPos == null)
        {
            _nextPos = transform.position;
        }
        return _pathfindingManager.FindPath(_nextPos, _map.goalCell.transform.position);
    }

    public void OnSignalReceived(NewTowerSignal signal)
    {
        if (!_isFlying && _path.Contains(signal.position))
        {
            MoveOnPath(RecalculatePath());
        }
    }

    public void OnSignalReceived(GameLostSignal signal)
    {
        Destroy(gameObject);
    }

    public void OnSignalReceived(TowerSoldSignal signal)
    {
        if (!_isFlying)
        {
            MoveOnPath(RecalculatePath());
        }
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<GameLostSignal>(this);
        Signalbus.Unsubscribe<TowerSoldSignal>(this);
        Signalbus.Unsubscribe<NewTowerSignal>(this);
    }

    public int GetLives()
    {
        return _lives;
    }

    public int GetMaxLives()
    {
        return _maxLives;
    }

    public int GetArmor()
    {
        return _armor;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public int GetBounty()
    {
        return _bounty;
    }

    public string GetName()
    {
        return _enemyName;
    }

    public List<string> GetSpecials()
    {
        List<string> output = new List<string>();
        if (_isFlying)
        {
            output.Add("Flying");
        }
        if(_isSplitting)
        {
            output.Add("Splitting up");
        }
        if (_isGold)
        {
            output.Add("High bounty");
            output.Add("No damage");
        }
        if (_isScaling)
        {
            output.Add("Stronger each round");
        }
        if (output.Count == 0)
        {
            output.Add("None");
        }
        return output;
    }

    public void ApplySlow(IceTower iceTower, float slow)
    {
        Debug.Log($"ApplySlow tower = {iceTower}, slow = {slow}");
        if(slow < _slow)
        {
            _slow = slow;
        }
        _slowedBy.Add((iceTower, slow));
    }

    public void RemoveSlow(IceTower iceTower, float slow)
    {
        Debug.Log($"RemoveSlow tower = {iceTower}, slow = {slow}");
        _slowedBy.Remove((iceTower, slow));
        if(slow == _slow)
        {

            if (_slowedBy.Count == 0)
            {
                _slow = 1.0f;
            }
            else
            {
                _slow = _slowedBy.Min(t => t.Item2);
            }
        }
    }

    public void ApplyBurn(int dmg, int stacks)
    {
        _burnDmg = Mathf.Max(_burnDmg, dmg);
        _burnStacks += stacks;
    }

    public int GetBurnStacks()
    {
        return _burnStacks;
    }

    public void SetIsPush(bool isPushed)
    {
        _isPushed = isPushed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }
}
