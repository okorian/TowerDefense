using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemysSignal : ISignal
{
    public Vector2 position;
    public Vector2[] path;
    public int count;
    public int liveMult;
    public int armorBuff;
    public int speedBuff;
}
