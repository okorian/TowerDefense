using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour, ISubscriber<ClearRuinSignal>, ISubscriber<RestartGameSignal>
{
    int _x;
    int _y;
    string _towerName;

    void Start()
    {
        Signalbus.Subscirbe<ClearRuinSignal>(this);
        Signalbus.Subscirbe<RestartGameSignal>(this);
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
        Signalbus.Unsubscribe<ClearRuinSignal>(this);
        Signalbus.Unsubscribe<RestartGameSignal>(this);
    }

    public void OnSignalReceived(ClearRuinSignal signal)
    {
        Map.Instance.RemoveTower(_x, _y);
        Signalbus.Fire<TowerSoldSignal>(new TowerSoldSignal() { towerName = _towerName });
        Destroy(gameObject);
    }

    public void OnSignalReceived(RestartGameSignal signal)
    {
        Destroy(gameObject);
    }
}
