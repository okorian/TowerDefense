using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplaySelection : MonoBehaviour
{
    Tower _selectedTower;
    Enemy _selectedEnemy;

    [SerializeField] GameObject _towerPanel;
    [SerializeField] TextMeshProUGUI _towerNameTMP;
    [SerializeField] TextMeshProUGUI _towerLvlTMP;
    [SerializeField] TextMeshProUGUI _towerDmgTMP;
    [SerializeField] TextMeshProUGUI _towerAtkSpeedTMP;
    [SerializeField] TextMeshProUGUI _towerDPSTMP;
    [SerializeField] TextMeshProUGUI _towerRangeTMP;
    [SerializeField] TextMeshProUGUI _towerUpgradeCostTMP;
    [SerializeField] TextMeshProUGUI _towerRefundValueTMP;
    [SerializeField] GameObject _towerUpgradeBtnTMP;

    [SerializeField] GameObject _enemyPanel;
    [SerializeField] TextMeshProUGUI _enemyNameTMP;
    [SerializeField] TextMeshProUGUI _enemyLivesTMP;
    [SerializeField] TextMeshProUGUI _enemyArmorTMP;
    [SerializeField] TextMeshProUGUI _enemySpeedTMP;
    [SerializeField] TextMeshProUGUI _enemyBountyTMP;
    [SerializeField] TextMeshProUGUI _enemySpezialTMP;

    private void Update()
    {
        if(_selectedEnemy != null)
        {
            _enemyLivesTMP.text = $"{_selectedEnemy.GetLives()} / {_selectedEnemy.GetMaxLives()}";
        }
        else
        {
            try
            {
                _enemyLivesTMP.text = $"{0} / {_enemyLivesTMP.text.Split("/ ")[1]}";
            }
            catch (IndexOutOfRangeException ex)
            {

            }
        }
    }

    public void DisplayTower()
    {
        if (_selectedTower == null) return;

        _towerNameTMP.text = _selectedTower.GetName();
        _towerLvlTMP.text = $"{_selectedTower.GetLvl() + 1}";
        _towerDmgTMP.text = $"{_selectedTower.GetDmg()}";
        _towerAtkSpeedTMP.text = $"{_selectedTower.GetAttackSpeed():F2}";
        _towerDPSTMP.text = $"{(_selectedTower.GetDmg() * _selectedTower.GetAttackSpeed()):F2}";
        _towerRangeTMP.text = $"{(int) _selectedTower.GetRange()}";
        if (_selectedTower.GetLvl() >= 4)
        {
            _towerUpgradeBtnTMP.SetActive(false);
        }
        else
        {
            _towerUpgradeCostTMP.text = $"{_selectedTower.GetPrice()}";
            _towerUpgradeBtnTMP.SetActive(true);
        }
        _towerRefundValueTMP.text = $"{_selectedTower.GetRefund()}";
        _selectedTower.SetRangeIndicatorActive(true);
        _towerPanel.SetActive(true);
    }

    public void DisplayEnemy()
    {
        if (_selectedEnemy == null) return;

        _enemyNameTMP.text = _selectedEnemy.GetName();
        _enemyLivesTMP.text = $"{_selectedEnemy.GetLives()} / {_selectedEnemy.GetMaxLives()}";
        _enemyArmorTMP.text = $"{_selectedEnemy.GetArmor()}";
        _enemySpeedTMP.text = $"{_selectedEnemy.GetSpeed()}";
        _enemyBountyTMP.text = $"{_selectedEnemy.GetBounty()}";
        string specials = "";
        foreach(string s in _selectedEnemy.GetSpecials())
        {
            specials += s + '\n';
        }
        _enemySpezialTMP.text = specials;
        _enemyPanel.SetActive(true);
    }

    public void UpgradeTower()
    {
        if (_selectedTower == null) return;

        _selectedTower.Upgrade();
        DisplayTower();
    }

    public void SellTower()
    {
        if (_selectedTower == null) return;

        _selectedTower.Sell();
        _towerPanel.SetActive(false);
        _selectedTower = null;
    }

    public void Select(GameObject selected)
    {
        if (_selectedTower != null)
        {
            _selectedTower.SetRangeIndicatorActive(false);
        }

        if(selected == null)
        {
            _towerPanel.SetActive(false);
            _enemyPanel.SetActive(false);
        }
        else if (selected.CompareTag("Tower"))
        {
            _selectedTower = selected.GetComponent<Tower>();
            _selectedEnemy = null;
            _enemyPanel.SetActive(false);
            DisplayTower();
        }
        else if (selected.CompareTag("Enemy"))
        {
            _selectedTower = null;
            _selectedEnemy = selected.GetComponent<Enemy>();
            _towerPanel.SetActive(false);
            DisplayEnemy();
        }
    }
}
