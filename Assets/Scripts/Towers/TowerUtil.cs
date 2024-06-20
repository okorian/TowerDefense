using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUtil
{
    public static LayerMask enemyLayer = LayerMask.GetMask("Enemy");

    public static bool IsTargetInRange(Vector3 position, Enemy target, float range)
    {
        return target != null && Vector2.Distance(position, target.transform.position) <= range * 1.5f;
    }

    public static Enemy FindNearestTarget(Vector3 position, float range)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(position, range * 1.5f, enemyLayer);
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;

        foreach (var hitCollider in enemiesInRange)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float distance = Vector2.Distance(position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    public static Enemy FindFireTarget(Vector3 position, float range)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(position, range * 1.5f, enemyLayer);
        int minBurnStacks = int.MaxValue;
        Enemy targetEnemy = null;

        foreach (var hitCollider in enemiesInRange)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                int burnStacks = enemy.GetBurnStacks();

                if (burnStacks == 0)
                {
                    return targetEnemy;
                }

                if (burnStacks < minBurnStacks)
                {
                    minBurnStacks = burnStacks;
                    targetEnemy = enemy;
                }
            }
        }

        return targetEnemy;
    }

    public static List<Enemy> FindTargetsInRange(Vector3 position, float range)
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(position, range * 1.5f, enemyLayer);
        List<Enemy> enemys = new List<Enemy>();

        foreach (var hitCollider in enemiesInRange)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemys.Add(enemy);
            }
        }

        return enemys;
    }

    public static void UpdateRangeIndicator(LineRenderer lineRenderer, float range)
    {
        int segments = 50;
        float angle = 360f / segments;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angle);
            float x = Mathf.Cos(rad) * range;
            float y = Mathf.Sin(rad) * range;
            points[i] = new Vector3(x, y, -2f);
        }

        lineRenderer.SetPositions(points);
    }
}
