using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public string player;
    public float playTime;
    public List<RoundData> rounds = new List<RoundData>();
}

[Serializable]
public class RoundData
{
    public int roundNumber;
    public float roundTime;
    public bool hasLost;
    public int livesLost;
    public int livesLeft;
    public int goldSpent;
    public int goldEarned;
    public int enemiesDefeated;
    public int goldEnemiesSpawned;
    public int goldEnemiesDefeated;
    public int arrowTowersPlaced;
    public int cannonTowersPlaced;
    public int fireTowersPlaced;
    public int iceTowersPlaced;
    public int goldTowersPlaced;
    public int arrowTowersSold;
    public int cannonTowersSold;
    public int fireTowersSold;
    public int iceTowersSold;
    public int goldTowersSold;
    public int[] arrowTowersUpgraded;
    public int[] cannonTowersUpgraded;
    public int[] fireTowersUpgraded;
    public int[] iceTowersUpgraded;
    public int[] goldTowersUpgraded;
    public int boomButtonUsed;
    public int boomButtonKills;
}