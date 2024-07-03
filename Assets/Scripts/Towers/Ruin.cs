using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour, ISubscriber<WaveFinishedSignal>
{
    int _x;
    int _y;
    string _towerName;

    void Start()
    {
        Signalbus.Subscirbe<WaveFinishedSignal>(this);
    }

    public void Initialize(int x, int y, string towerName)
    {
        _x = x;
        _y = y;
        _towerName = towerName;
        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<WaveFinishedSignal>(this);
    }

    public void OnSignalReceived(WaveFinishedSignal signal)
    {
        Map.Instance.RemoveTower(_x, _y);
        Signalbus.Fire<TowerSoldSignal>(new TowerSoldSignal() { towerName = _towerName });
        Destroy(gameObject);
    }
}
