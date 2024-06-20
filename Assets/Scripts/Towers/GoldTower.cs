using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldTower : Tower, ISubscriber<GameLostSignal>, ISubscriber<RestartGameSignal>
{
    bool _gameLost = false;

    private void Update()
    {
        if (!_gameLost)
        {
            _attackTimer += Time.deltaTime;

            if (_attackTimer >= _attackSpeed[_lvl])
            {
                _gameController.EarnGold(_dmg[_lvl]);
                _attackTimer = 0f;
            }
        }
    }

    public override void Attack(Enemy enemy)
    {

    }

    public override void Initialize(int x, int y, TowerData data)
    {
        base.Initialize(x, y, data);

        Signalbus.Subscirbe<GameLostSignal>(this);
        Signalbus.Subscirbe<RestartGameSignal>(this);
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<GameLostSignal>(this);
        Signalbus.Unsubscribe<RestartGameSignal>(this);
    }

    public override void SetRangeIndicatorActive(bool active)
    {

    }

    public void OnSignalReceived(GameLostSignal signal)
    {
        _gameLost = true;
    }
}
