using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int size;
    Stack<GameObject> pool = new Stack<GameObject>();

    private void Start()
    {
        while(pool.Count < size)
        {
            ReturnToPool(Spawn());
        }
    }

    private GameObject Spawn()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.parent = gameObject.transform;
        return projectile;
    }

    public GameObject Spawn(Enemy target, Tower tower)
    {
        GameObject projectile = pool.Any() ? pool.Pop() : Instantiate(projectilePrefab, tower.transform.position, Quaternion.identity);
        projectile.transform.parent = gameObject.transform;
        ResetInstance(projectile);
        projectile.GetComponent<Projectile>().Initialize(target, tower, this);
        projectile.SetActive(true);
        return projectile;
    }

    public void ResetInstance(GameObject projectile)
    {
        projectile.GetComponent<Projectile>().Reset();
    }

    public void ReturnToPool(GameObject projectile)
    {
        projectile.SetActive(false);
        pool.Push(projectile);
    }
}
