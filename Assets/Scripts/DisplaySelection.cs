using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.TMP_Dropdown;

public class DisplaySelection : MonoBehaviour
{
    Tower _selectedTower;
    Enemy _selectedEnemy;

    [SerializeField] GameObject _towerPanel;
    [SerializeField] GameObject _towerValuePanel;
    [SerializeField] TextMeshProUGUI _goldTowerTMP;
    [SerializeField] TextMeshProUGUI _towerNameTMP;
    [SerializeField] TextMeshProUGUI _towerLvlTMP;
    [SerializeField] TextMeshProUGUI _towerDmgTMP;
    [SerializeField] TextMeshProUGUI _towerAtkSpeedTMP;
    [SerializeField] TextMeshProUGUI _towerDPSTMP;
    [SerializeField] TextMeshProUGUI _towerRangeTMP;
    [SerializeField] TextMeshProUGUI _towerUpgradeCostTMP;
    [SerializeField] TextMeshProUGUI _towerRefundValueTMP;
    [SerializeField] GameObject _towerSpecial;
    [SerializeField] TextMeshProUGUI _towerSpecialValueTMP;
    [SerializeField] TMP_Dropdown _targetModeDropdown;
    [SerializeField] GameObject _towerUpgradeBtnTMP;

    [SerializeField] GameObject _enemyPanel;
    [SerializeField] TextMeshProUGUI _enemyNameTMP;
    [SerializeField] TextMeshProUGUI _enemyLivesTMP;
    [SerializeField] TextMeshProUGUI _enemyArmorTMP;
    [SerializeField] TextMeshProUGUI _enemySpeedTMP;
    [SerializeField] TextMeshProUGUI _enemyBountyTMP;
    [SerializeField] TextMeshProUGUI _enemyBurnTMP;
    [SerializeField] TextMeshProUGUI _enemySpezialTMP;

    [SerializeField] TooltipManager _toolTipManager;

    private void Update()
    {
        if(_selectedEnemy != null)
        {
            _enemyLivesTMP.text = $"{_selectedEnemy.GetLives()} / {_selectedEnemy.GetMaxLives()}";
            _enemyBurnTMP.text = $"{_selectedEnemy.GetBurnStacks()}";
        }
        else
        {
            try
            {
                _enemyLivesTMP.text = $"0 / {_enemyLivesTMP.text.Split("/ ")[1]}";
                _enemyBurnTMP.text = $"0";
            }
            catch (IndexOutOfRangeException ex)
            {

            }
        }

        if(_selectedTower != null)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                UpgradeTower();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SellTower();
            }
        }
    }

    public void DisplayTower()
    {
        if (_selectedTower == null) return;

        _towerNameTMP.text = _selectedTower.GetName();

        if (_selectedTower.IsGold())
        {
            _goldTowerTMP.text = $"This tower cannot cause any damage." +
                $"\nIt produces {_selectedTower.GetDmg()} gold" +
                $"\nevery {_selectedTower.GetAttackSpeedValue():F0} seconds.";
            _goldTowerTMP.gameObject.SetActive(true);
            _towerValuePanel.SetActive(false);
        }
        else
        {
            _towerLvlTMP.text = $"{_selectedTower.GetLvl() + 1}";
            _towerDmgTMP.text = $"{_selectedTower.GetDmg()}";
            _towerAtkSpeedTMP.text = $"{_selectedTower.GetAttackSpeed():F2}";
            _towerDPSTMP.text = $"{(_selectedTower.GetDmg() * _selectedTower.GetAttackSpeed()):F2}";
            _towerRangeTMP.text = $"{(int)_selectedTower.GetRange()}";

            if (_selectedTower.IsFire())
            {
                _towerSpecialValueTMP.text = $"Add {_selectedTower.GetBurnStacks()} burn stacks" +
                    $"\nfor {_selectedTower.GetBurnDMG()} damage each" +
                    $"\nBurn ignores armor";
                _towerSpecialValueTMP.gameObject.SetActive(true);
                _towerSpecial.SetActive(true);
            }

            if (_selectedTower.IsIce())
            {
                _towerSpecialValueTMP.text = $"Hits all enemies" +
                    $"\nApplys {((1 - _selectedTower.GetSlow()) * 100):F0}% slow";
                _towerSpecialValueTMP.gameObject.SetActive(true);
                _towerSpecial.SetActive(true);
            }

            if (!_selectedTower.IsFire() && !_selectedTower.IsIce())
            {
                _towerSpecialValueTMP.gameObject.SetActive(false);
                _towerSpecial.SetActive(false);
            }

            _selectedTower.SetRangeIndicatorActive(true);
            SetTargetOptions();
            _goldTowerTMP.gameObject.SetActive(false);
            _towerValuePanel.SetActive(true);
        }

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
        _enemyBurnTMP.text = $"{_selectedEnemy.GetBurnStacks()}";
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

        _selectedTower.Sell(_selectedTower.GetName());
        _towerPanel.SetActive(false);
        _selectedTower = null;
        _toolTipManager.HideTooltip();
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

    public void SetTargetOptions()
    {
        if(_selectedTower != null)
        {
            _targetModeDropdown.onValueChanged.RemoveAllListeners();
            _targetModeDropdown.options.Clear();

            _targetModeDropdown.options.Add(new OptionData("Nearest enemy"));
            _targetModeDropdown.options.Add(new OptionData("Lowest lives"));
            _targetModeDropdown.options.Add(new OptionData("Highest lives"));
            _targetModeDropdown.options.Add(new OptionData("Lowest armor"));
            _targetModeDropdown.options.Add(new OptionData("Highest armor"));

            if (_selectedTower.IsFire())
            {
                _targetModeDropdown.options.Add(new OptionData("Not burning"));
            }

            _targetModeDropdown.value = _selectedTower.GetTargetMode();

            _targetModeDropdown.onValueChanged.AddListener(_selectedTower.SetTargetMode);
        }
    }
}
