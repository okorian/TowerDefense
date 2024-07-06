using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour, ISubscriber<EnemyDiedSignal>, ISubscriber<EnemyReachedGoalSignal>, ISubscriber<EnemySpawnedSignal>, ISubscriber<WaveFinishedSignal>, ISubscriber<NewTowerSignal>
{
    public static GameController Instance;

    [SerializeField] GameObject _gameLostPanel;
    [SerializeField] TextMeshProUGUI _roundTMP;
    [SerializeField] TextMeshProUGUI _goldTMP;
    [SerializeField] TextMeshProUGUI _enemysTMP;
    [SerializeField] TextMeshProUGUI _livesTMP;
    [SerializeField] Transform _enemyManager;
    [SerializeField] TimeScale _timeScale;

    [SerializeField] int _gold;

    int _round;
    int _buff;
    int _lives;
    int _enemyCount;
    bool _roundIsRunning;
    bool _roundHasFinished;
    bool _gameLost;
    bool _firstTowerPlaced;
    bool _hasPlayedSound;
    bool _hasClearedRuins;
    //float _interest;
    //float _interestTimer;
    float _goldTimer;
    float _pauseTimer;

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
    }

    private void Start()
    {
        _round = 0;
        _buff = 0;
        _lives = 20;
        if(_gold == 0)
        {
            _gold = 150;
        }
        //_interest = 0.1f;
        _roundIsRunning = false;
        _roundHasFinished = true;
        _gameLost = false;
        _firstTowerPlaced = false;
        _hasPlayedSound = false;
        _hasClearedRuins = false;

        _goldTimer = 0f;
        //_interestTimer = 0f;
        _pauseTimer = 0f;

        _roundTMP.text = "Round: " + _round;
        _goldTMP.text = "Gold: " + _gold;
        _livesTMP.text = "Lives: " + _lives;
        _enemysTMP.text = "Enemys: " + _enemyCount;

        Signalbus.Subscirbe<EnemyDiedSignal>(this);
        Signalbus.Subscirbe<EnemyReachedGoalSignal>(this);
        Signalbus.Subscirbe<EnemySpawnedSignal>(this);
        Signalbus.Subscirbe<WaveFinishedSignal>(this);
        Signalbus.Subscirbe<NewTowerSignal>(this);
    }

    private void Update()
    {
        if (_gameLost || !_firstTowerPlaced)
        {
            return;
        }

        _goldTimer += Time.deltaTime;
        //_interestTimer += Time.deltaTime;

        if (_goldTimer >= 1)
        {
            EarnGold(1);
            _goldTimer = 0f;
            _goldTMP.text = "Gold: " + _gold;
        }

        /*
        if (_interestTimer >= 30)
        {
            EarnGold((int) (_gold * _interest));
            _interestTimer = 0f;
        }
        */

        _enemyCount = _enemyManager.childCount;
        _enemysTMP.text = "Enemys: " + _enemyCount;

        if (_enemyCount <= 0 )
        {
            _roundIsRunning = false;
        }
        else
        {
            _roundIsRunning = true;
        }

        if (!_roundIsRunning && _roundHasFinished)
        {
            if (!_hasClearedRuins)
            {
                Signalbus.Fire<ClearRuinSignal>(new ClearRuinSignal());
                _hasClearedRuins = true;
            }

            _pauseTimer += Time.deltaTime;

            if(_pauseTimer >= 8 && !_hasPlayedSound)
            {
                Signalbus.Fire<PlaySoundSignal>(new PlaySoundSignal() { clipName = "newRound" });
                _hasPlayedSound = true;
            }

            if(_pauseTimer >= 10)
            {
                _pauseTimer = 0;
                _round++;
                if(_round % 5 == 0)
                {
                    _buff++;
                }
                _roundTMP.text = "Round: " + _round;

                Signalbus.Fire<SpawnEnemyWaveSignal>(new SpawnEnemyWaveSignal() { round = _round, buff = _buff });

                _roundIsRunning = true;
                _roundHasFinished = false;
                _hasPlayedSound = false;
                _hasClearedRuins = false;
            }
        }
    }

    public int GetRound()
    {
        return _round;
    }

    public void EarnGold(int gold)
    {
        if (_gold > int.MaxValue - gold)
        {
            _gold = int.MaxValue;
        }
        else
        {
            _gold += gold;
        }
        _goldTMP.text = "Gold: " + _gold;

        Signalbus.Fire<GoldEarnedSignal>(new GoldEarnedSignal() { gold = gold });
    }

    public bool PayGold(int gold)
    {
        if (_gold >= gold)
        {
            _gold -= gold;
            _goldTMP.text = "Gold: " + _gold;
            Signalbus.Fire<GoldSpentSignal>(new GoldSpentSignal() { gold = gold });
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GameLost()
    {
        Signalbus.Fire<GameLostSaveSignal>(new GameLostSaveSignal());
        Signalbus.Fire<GameLostSignal>(new GameLostSignal());
        _roundIsRunning = false;
        _roundHasFinished = false;
        _hasPlayedSound = false;
        _hasClearedRuins = false;
        _gameLost = true;
        _gameLostPanel.SetActive(true);
    }

    public void OnSignalReceived(EnemyDiedSignal signal)
    {
        EarnGold(signal.value);
    }

    public void OnSignalReceived(EnemyReachedGoalSignal signal)
    {
        _lives--;
        _livesTMP.text = "Lives: " + _lives;
        if (_lives <= 0)
        {
            GameLost();
        }
    }

    public void OnSignalReceived(EnemySpawnedSignal signal)
    {
        _enemyCount++;
        _enemysTMP.text = "Enemys: " + _enemyCount;
    }

    public void OnSignalReceived(WaveFinishedSignal signal)
    {
        _roundHasFinished = true;
    }

    public void RestartGame()
    {
        _round = 0;
        _buff = 0;
        _lives = 20;
        _gold = 150;
        //_interest = 0.1f;
        _roundIsRunning = false;
        _roundHasFinished = true;
        _gameLost = false;
        _firstTowerPlaced = false;
        _hasPlayedSound = false;
        _hasClearedRuins = false;

         _goldTimer = 0f;
        //_interestTimer = 0f;
        _pauseTimer = 0f;
        _enemyCount = 0;

        _roundTMP.text = "Round: " + _round;
        _goldTMP.text = "Gold: " + _gold;
        _livesTMP.text = "Lives: " + _lives;
        _enemysTMP.text = "Enemys: " + _enemyCount;

        _timeScale.ScaleTime(1f);

        Debug.Log($"Fire restart Game Signal");
        Signalbus.Fire<RestartGameSignal>(new RestartGameSignal());
        _gameLostPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<EnemyDiedSignal>(this);
        Signalbus.Unsubscribe<EnemyReachedGoalSignal>(this);
        Signalbus.Unsubscribe<EnemySpawnedSignal>(this);
        Signalbus.Unsubscribe<WaveFinishedSignal>(this);
        Signalbus.Unsubscribe<NewTowerSignal>(this);
    }

    public void ToMenu()
    {
        RestartGame();
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OnSignalReceived(NewTowerSignal signal)
    {
        if(!_firstTowerPlaced)
        {
            _timeScale.ResumeTime();
            _firstTowerPlaced = true;
            Signalbus.Fire<ActivateShockwaveSignal>(new ActivateShockwaveSignal());
        }
    }

    public int GetLives()
    {
        return _lives;
    }

    public int GetGold()
    {
        return _gold;
    }
}
