using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceTower : Tower, ISubscriber<RestartGameSignal>
{
    List<Enemy> _lastTargets = new List<Enemy>();
    [SerializeField] LineRenderer _lineRenderer;
    ParticleSystem _particle;
    float[] _particleLifetime = new float[] { 1, 2.75f, 2.75f, 4, 4 };
    float[] _slow = new float[] { 0.95f, 0.9f, 0.85f, 0.8f, 0.75f };

    private void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackSpeed[_lvl])
        {
            Attack(null);
        }
    }

    public override void Initialize(int x, int y, TowerData data)
    {
        base.Initialize(x, y, data);

        _lineRenderer.useWorldSpace = false;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 51;
        TowerUtil.UpdateRangeIndicator(_lineRenderer, _range[_lvl]);
        _lineRenderer.enabled = false;
        _particle = gameObject.GetComponentInChildren<ParticleSystem>();

        Signalbus.Subscirbe<RestartGameSignal>(this);
    }

    public override void Attack(Enemy enemyNotUsed)
    {
        List<Enemy> currentTargets = TowerUtil.FindTargetsInRange(transform.position, _range[_lvl]);
        if (currentTargets.Count > 0)
        {
            foreach (Enemy enemy in currentTargets)
            {
                enemy.TakeDamage(_dmg[_lvl]);
                enemy.ApplySlow(this, _slow[_lvl]);
            }
            _attackTimer = 0;
        }
        Debug.Log($"_lastTargets = {_lastTargets}, currentTargets = {currentTargets}");
        foreach(Enemy enemy in _lastTargets.Except(currentTargets).ToList())
        {
            if (enemy != null)
            { 
                enemy.RemoveSlow(this, _slow[_lvl]); 
            }
        }
        _lastTargets = currentTargets;
    }

    public override bool Upgrade()
    {
        if (_lvl < 4 && _gameController.PayGold(_price[_lvl + 1]))
        {
            List<Enemy> currentTargets = TowerUtil.FindTargetsInRange(transform.position, _range[_lvl]);
            foreach (Enemy enemy in _lastTargets.Except(currentTargets).ToList())
            {
                if (enemy != null)
                {
                    enemy.RemoveSlow(this, _slow[_lvl]);
                }
            }

            Signalbus.Fire<TowerUpgradeSignal>(new TowerUpgradeSignal() { towerName = GetName(), lvl = _lvl });
            _lvl++;
            UpdateLvlSprite();
            TowerUtil.UpdateRangeIndicator(_lineRenderer, _range[_lvl]);
            _particle.startLifetime = _particleLifetime[_lvl];

            foreach (Enemy enemy in currentTargets)
            {
                enemy.ApplySlow(this, _slow[_lvl]);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public override void SetRangeIndicatorActive(bool active)
    {
        _lineRenderer.enabled = active;
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<RestartGameSignal>(this);
    }

    public override float GetSlow()
    {
        return _slow[_lvl];
    }
}
