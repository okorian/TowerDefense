using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Transform _targetTransform;
    private Enemy _enemy;
    private Tower _tower;
    private ProjectileFactory _projectileFactory;

    public void Initialize(Enemy enemy, Tower tower, ProjectileFactory projectileFactory)
    {
        gameObject.transform.position = new Vector3(tower.transform.position.x, tower.transform.position.y, enemy.transform.position.z);
        _enemy = enemy;
        _tower = tower;
        _targetTransform = _enemy.transform;
        _projectileFactory = projectileFactory;
    }

    public void Reset()
    {
        _enemy = null;
        _tower = null;
        _targetTransform = null;
        _projectileFactory = null;
    }

    void Update()
    {
        if (_targetTransform != null && _enemy != null)
        {
            try
            {
                Vector3 direction = (_targetTransform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                //transform.LookAt(_targetTransform, Vector3.back); // Rotation stimmt nicht deswegen unsichtbar
                RotateTowardsTarget();

                if (Vector3.Distance(transform.position, _targetTransform.position) < 0.1f)
                {
                    _tower.Attack(_enemy);
                    _projectileFactory.ReturnToPool(gameObject);
                }
            }
            catch (System.NullReferenceException)
            {
                _projectileFactory.ReturnToPool(gameObject);
            }
        }
        else
        {
            _projectileFactory.ReturnToPool(gameObject);
        }
    }

    private void RotateTowardsTarget()
    {
        Vector2 direction = _targetTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
