using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetUp : MonoBehaviour
{
    [SerializeField] TowerData _arrowTower;
    [SerializeField] TowerData _cannonTower;
    [SerializeField] TowerData _fireTower;
    [SerializeField] TowerData _goldTower;
    [SerializeField] TowerData _iceTower;

    [SerializeField] TextMeshProUGUI _arrowTowerTMP;
    [SerializeField] TextMeshProUGUI _cannonTowerTMP;
    [SerializeField] TextMeshProUGUI _fireTowerTMP;
    [SerializeField] TextMeshProUGUI _goldTowerTMP;
    [SerializeField] TextMeshProUGUI _iceTowerTMP;

    private void Start()
    {
        _arrowTowerTMP.text = _arrowTower.price[0] + " gold";
        _cannonTowerTMP.text = _cannonTower.price[0] + " gold";
        _fireTowerTMP.text = _fireTower.price[0] + " gold";
        _goldTowerTMP.text = _goldTower.price[0] + " gold";
        _iceTowerTMP.text = _iceTower.price[0] + " gold";
    }
}
