using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] List<Sprite> levelSprites;
    [SerializeField] SpriteRenderer levelSpriteRenderer;

    protected int _x;
    protected int _y;
    protected int _lvl;
    protected int[] _price;
    protected int[] _dmg;
    protected float[] _range;
    protected float[] _attackSpeed;
    protected string _towerName;
    protected GameController _gameController;

    protected float _attackTimer;

    public virtual void Initialize(int x, int y, TowerData data)
    {
        GetComponent<SpriteRenderer>().sprite = data.towerSprite;

        _x = x;
        _y = y;
        _lvl = 0;
        _price = data.price;
        _dmg = data.dmg;
        _range = data.range;
        _attackSpeed = data.attackSpeed;
        _towerName = data.towerName;
        UpdateLvlSprite();

        _gameController = GameController.Instance;
        _attackTimer = _attackSpeed[0];

        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }

    public virtual bool Upgrade()
    {
        if (_lvl < 5 && _gameController.PayGold(_price[_lvl + 1]))
        {
            _lvl++;
            UpdateLvlSprite();
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void UpdateLvlSprite()
    {
        levelSpriteRenderer.sprite = levelSprites[_lvl];
    }

    public abstract void Attack(Enemy enemy);

    public void Sell()
    {
        _gameController.EarnGold(GetRefund());

        Map.Instance.RemoveTower(_x, _y);
        Signalbus.Fire<TowerSoldSignal>(new TowerSoldSignal());
        Destroy(gameObject);
    }

    public void OnSignalReceived(RestartGameSignal signal)
    {
        Destroy(gameObject);
    }

    public int GetLvl()
    {
        return _lvl;
    }

    public int GetDmg()
    {
        if (_lvl >= _dmg.Length)
        {
            return -1;
        }
        return _dmg[_lvl];
    }

    public float GetAttackSpeed()
    {
        if (_lvl >= _attackSpeed.Length)
        {
            return -1;
        }
        return 1f / _attackSpeed[_lvl];
    }

    public float GetRange()
    {
        if (_lvl >= _range.Length)
        {
            return -1;
        }
        return _range[_lvl];
    }

    public int GetPrice()
    {
        if (_lvl >= _price.Length - 1)
        {
            return -1;
        }
        return _price[_lvl + 1];
    }

    public int GetRefund()
    {
        if (_lvl >= _price.Length)
        {
            return -1;
        }
        int refund = 0;
        for (int i = 0; i <= _lvl; i++)
        {
            refund += _price[i];
        }
        return refund / 2;
    }

    public string GetName()
    {
        return _towerName;
    }

    public abstract void SetRangeIndicatorActive(bool active);
}
