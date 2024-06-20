using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Tower Defense/Enemy")]

public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int lives;
    public int armor;
    public int value;
    public float speed;
    public GameObject enemyPrefab;
    public bool isFlying;
    public bool isSplitting;
    public bool isGold;
    public bool isScaling;
}
