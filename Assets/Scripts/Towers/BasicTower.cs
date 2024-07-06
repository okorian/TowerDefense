using UnityEngine;

public class BasicTower : Tower, ISubscriber<RestartGameSignal>
{
    protected Enemy _currentTarget;
    [SerializeField] LineRenderer _lineRenderer;
    ProjectileFactory _projectileFactory;

    private void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackSpeed[_lvl])
        {
            if (_currentTarget == null || !TowerUtil.IsTargetInRange(transform.position, _currentTarget, _range[_lvl]))
            {
                _currentTarget = FindTarget();
            }

            if (_currentTarget != null)
            {
                _projectileFactory.Spawn(_currentTarget, this);
                _attackTimer = 0;
            }

        }
    }

    public override void Initialize(int x, int y, TowerData data)
    {
        base.Initialize(x, y, data);
        _currentTarget = null;

        _lineRenderer.useWorldSpace = false;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.positionCount = 51;
        TowerUtil.UpdateRangeIndicator(_lineRenderer, _range[_lvl]);
        _lineRenderer.enabled = false;

        if (IsCannon())
        {
            _projectileFactory = GameObject.FindWithTag("CannonProjectileFactory").GetComponent<ProjectileFactory>();
        }
        else
        {
            _projectileFactory = GameObject.FindWithTag("ArrowProjectileFactory").GetComponent<ProjectileFactory>();
        }

        SetTargetMode(0);

        Signalbus.Subscirbe<RestartGameSignal>(this);
    }

    public override void Attack(Enemy enemy)
    {
        if (enemy != null)
        {
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
        else { 
            return false; 
        }
    }

    public override void SetRangeIndicatorActive(bool active)
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = active;
        }
    }
}
