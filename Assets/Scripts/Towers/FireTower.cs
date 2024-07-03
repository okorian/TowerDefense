using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : Tower, ISubscriber<RestartGameSignal>
{
    [SerializeField] LineRenderer _lineRenderer;
    int[] _burnDmg = new int[] { 1, 2, 2, 3, 3 };
    int[] _burnStacks = new int[] { 2, 2, 3, 3, 4 };
    ProjectileFactory _projectileFactory;

    public override void Initialize(int x, int y, TowerData data)
    {
        base.Initialize(x, y, data);

        _lineRenderer.useWorldSpace = false;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 51;
        TowerUtil.UpdateRangeIndicator(_lineRenderer, _range[_lvl]);
        _lineRenderer.enabled = false;

        _projectileFactory = data.projectileFactory;

        SetTargetMode(5);

        Signalbus.Subscirbe<RestartGameSignal>(this);
    }

    private void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackSpeed[_lvl])
        {
            Enemy enemy = FindTarget();

            if (enemy != null)
            {
                _projectileFactory.Spawn(enemy, this);
                _attackTimer = 0;
            }
        }
    }

    public override void Attack(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.ApplyBurn(_burnDmg[_lvl], _burnStacks[_lvl]);
            enemy.TakeDamage(_dmg[_lvl]);
        }
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<RestartGameSignal>(this);
    }

    public override bool Upgrade()
    {
        if (base.Upgrade())
        {
            TowerUtil.UpdateRangeIndicator(_lineRenderer, _range[_lvl]);
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

    public override int GetBurnStacks()
    {
        return _burnStacks[_lvl];
    }

    public override int GetBurnDMG()
    {
        return _burnDmg[_lvl];
    }
}
