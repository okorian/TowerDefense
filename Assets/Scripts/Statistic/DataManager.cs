using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;

public class DataManager : MonoBehaviour, ISubscriber<EnemyDiedOnShockwaveSignal>, ISubscriber<EnemyDiedSignal>, 
    ISubscriber<EnemyReachedGoalSignal>, ISubscriber<GameLostSaveSignal>, ISubscriber<NewTowerSignal>, ISubscriber<TowerSoldSignal>, 
    ISubscriber<WaveFinishedSignal>, ISubscriber<ShockwaveSignal>, ISubscriber<GoldEarnedSignal>, ISubscriber<GoldSpentSignal>, 
    ISubscriber<GoldEnemySpawnedSignal>, ISubscriber<GoldEnemyKilledSignal>
{
    [SerializeField] string _dataFilePath;
    [SerializeField] SendMail _sendMail;
    [SerializeField] TimeScale _timeScale;

    public string playerName;
    GameData _gameData;
    RoundData _currentRoundData;
    GameController _gameController;
    Map _map;
    float _roundTime;

    private void Start()
    {
        _dataFilePath = Path.Combine(Application.persistentDataPath, "GameData.json");

        _gameController = GameController.Instance;
        _map = Map.Instance;

        _gameData = new GameData();
        _currentRoundData = new RoundData();

        Signalbus.Subscirbe<EnemyDiedOnShockwaveSignal>(this);
        Signalbus.Subscirbe<EnemyDiedSignal>(this);
        Signalbus.Subscirbe<EnemyReachedGoalSignal>(this);
        Signalbus.Subscirbe<GameLostSaveSignal>(this);
        Signalbus.Subscirbe<NewTowerSignal>(this);
        Signalbus.Subscirbe<TowerSoldSignal>(this);
        Signalbus.Subscirbe<WaveFinishedSignal>(this);
        Signalbus.Subscirbe<ShockwaveSignal>(this);
        Signalbus.Subscirbe<GoldEarnedSignal>(this);
        Signalbus.Subscirbe<GoldSpentSignal>(this);
        Signalbus.Subscirbe<GoldEnemySpawnedSignal>(this);
        Signalbus.Subscirbe<GoldEnemyKilledSignal>(this);
    }

    private void OnDestroy()
    {
        SaveData();
        Signalbus.Unsubscribe<EnemyDiedOnShockwaveSignal>(this);
        Signalbus.Unsubscribe<EnemyDiedSignal>(this);
        Signalbus.Unsubscribe<EnemyReachedGoalSignal>(this);
        Signalbus.Unsubscribe<GameLostSaveSignal>(this);
        Signalbus.Unsubscribe<NewTowerSignal>(this);
        Signalbus.Unsubscribe<TowerSoldSignal>(this);
        Signalbus.Unsubscribe<WaveFinishedSignal>(this);
        Signalbus.Unsubscribe<ShockwaveSignal>(this);
        Signalbus.Unsubscribe<GoldEarnedSignal>(this);
        Signalbus.Unsubscribe<GoldSpentSignal>(this);
        Signalbus.Unsubscribe<GoldEnemySpawnedSignal>(this);
        Signalbus.Unsubscribe<GoldEnemyKilledSignal>(this);
    }

    private void Update()
    {
        _roundTime += Time.deltaTime;
    }

    public void SaveData()
    {
        try
        {
            _gameData.playTime = _timeScale.GetPlayTime();

            string jsonData = JsonUtility.ToJson(_gameData);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            // Vor dem Erstellen der neuen Datei die alte löschen, falls sie existiert
            if (File.Exists(_dataFilePath))
            {
                File.Delete(_dataFilePath);
            }

            // Neue Datei erstellen und Daten speichern
            using (FileStream fs = new FileStream(_dataFilePath, FileMode.CreateNew))
            {
                fs.Write(jsonBytes, 0, jsonBytes.Length);
            }

            Debug.Log("Data saved.");

            _sendMail.SendEmail(_dataFilePath);
        }
        catch (IOException ex)
        {
            Debug.Log($"Catch {ex.Message}");
        }
    }

    public void NewRound()
    {
        _currentRoundData.roundNumber = _gameController.GetRound();
        _currentRoundData.map = _map.GetMap();
        _currentRoundData.roundTime = _roundTime;
        _currentRoundData.livesLeft = _gameController.GetLives();
        _gameData.rounds.Add(_currentRoundData);
        _currentRoundData = new RoundData();
        _roundTime = 0f;
    }

    public void LostRound()
    {
        _currentRoundData.roundNumber = _gameController.GetRound();
        _currentRoundData.map = _map.GetMap();
        _currentRoundData.roundTime = _roundTime;
        _currentRoundData.livesLeft = _gameController.GetLives();
        _gameData.rounds.Add(_currentRoundData);
        _gameData.playTime = _timeScale.GetPlayTime();

        SaveData();

        _currentRoundData = new RoundData();
        _roundTime = 0f;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        _gameData.player = playerName;
    }

    public void OnSignalReceived(EnemyDiedOnShockwaveSignal signal)
    {
        _currentRoundData.boomButtonKills++;
    }

    public void OnSignalReceived(EnemyDiedSignal signal)
    {
        _currentRoundData.enemiesDefeated++;
    }

    public void OnSignalReceived(EnemyReachedGoalSignal signal)
    {
        _currentRoundData.livesLost++;
    }

    public void OnSignalReceived(GameLostSaveSignal signal)
    {
        _currentRoundData.hasLost = true;

        LostRound();
    }

    public void OnSignalReceived(NewTowerSignal signal)
    {
        switch (signal.towerName)
        {
            case "Arrow Tower":
                _currentRoundData.arrowTowersPlaced++;
                break;
            case "Cannon Tower":
                _currentRoundData.cannonTowersPlaced++;
                break;
            case "Fire Tower":
                _currentRoundData.fireTowersPlaced++;
                break;
            case "Ice Tower":
                _currentRoundData.iceTowersPlaced++;
                break;
            case "Gold Tower":
                _currentRoundData.goldTowersPlaced++;
                break;
        }

    }

    public void OnSignalReceived(TowerSoldSignal signal)
    {
        switch (signal.towerName)
        {
            case "Arrow Tower":
                _currentRoundData.arrowTowersSold++;
                break;
            case "Cannon Tower":
                _currentRoundData.cannonTowersSold++;
                break;
            case "Fire Tower":
                _currentRoundData.fireTowersSold++;
                break;
            case "Ice Tower":
                _currentRoundData.iceTowersSold++;
                break;
            case "Gold Tower":
                _currentRoundData.goldTowersSold++;
                break;
        }
    }

    public void OnSignalReceived(WaveFinishedSignal signal)
    {
        NewRound();
    }

    public void OnSignalReceived(ShockwaveSignal signal)
    {
        _currentRoundData.boomButtonUsed++;
    }

    public void OnSignalReceived(GoldEarnedSignal signal)
    {
        _currentRoundData.goldEarned += signal.gold;
    }

    public void OnSignalReceived(GoldSpentSignal signal)
    {
        _currentRoundData.goldSpent += signal.gold;
    }

    public void OnSignalReceived(GoldEnemySpawnedSignal signal)
    {
        _currentRoundData.goldEnemiesSpawned++;
    }

    public void OnSignalReceived(GoldEnemyKilledSignal signal)
    {
        _currentRoundData.goldEnemiesDefeated++;
    }
}
