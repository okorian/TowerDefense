using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldTower : Tower, ISubscriber<GameLostSignal>, ISubscriber<RestartGameSignal>
{
    [SerializeField] GameObject _money;
    Vector3 _originalPosition;

    bool _gameLost = false;

    private void Start()
    {
        _originalPosition = _money.transform.position;
    }

    private void Update()
    {
        if (!_gameLost)
        {
            _attackTimer += Time.deltaTime;

            if (_attackTimer >= _attackSpeed[_lvl])
            {
                _gameController.EarnGold(_dmg[_lvl]);
                _attackTimer = 0f;
                StartCoroutine(ShowMoneyAnimation());
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

    IEnumerator ShowMoneyAnimation()
    {
        _money.SetActive(true);

        Vector3 targetPosition = _money.transform.position + Vector3.up;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _money.transform.position = Vector3.Lerp(_originalPosition, targetPosition, t);
            yield return null;
        }

        _money.transform.position = _originalPosition;
        _money.SetActive(false);
    }
}
