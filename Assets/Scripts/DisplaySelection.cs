using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.TMP_Dropdown;

public class DisplaySelection : MonoBehaviour, ISubscriber<RestartGameSignal>
{
    List<Tower> _selectedTowers = new List<Tower>();
    Tower _selectedTower;
    Enemy _selectedEnemy;

    [SerializeField] GameObject _towerPanel;
    [SerializeField] GameObject _towerValuePanel;
    [SerializeField] TextMeshProUGUI _goldTowerTMP;
    [SerializeField] GameObject _towerCount;
    [SerializeField] TextMeshProUGUI _towerCountValueTMP;
    [SerializeField] TextMeshProUGUI _towerCountValueSingleTMP;
    [SerializeField] TextMeshProUGUI _towerNameTMP;
    [SerializeField] TextMeshProUGUI _towerLvlTMP;
    [SerializeField] TextMeshProUGUI _towerDmgTMP;
    [SerializeField] TextMeshProUGUI _towerAtkSpeedTMP;
    [SerializeField] TextMeshProUGUI _towerDPSTMP;
    [SerializeField] TextMeshProUGUI _towerRangeTMP;
    [SerializeField] TextMeshProUGUI _towerUpgradeCostTMP;
    [SerializeField] TextMeshProUGUI _towerUpgradeAllCostTMP;
    [SerializeField] TextMeshProUGUI _towerRefundValueTMP;
    [SerializeField] TextMeshProUGUI _towerRefundAllValueTMP;
    [SerializeField] GameObject _towerSpecial;
    [SerializeField] TextMeshProUGUI _towerSpecialValueTMP;
    [SerializeField] GameObject _targetMode;
    [SerializeField] TMP_Dropdown _targetModeDropdown;
    [SerializeField] GameObject _towerUpgradeBtn;
    [SerializeField] GameObject _towerUpgradeAllBtn;
    [SerializeField] GameObject _towerSellBtn;
    [SerializeField] GameObject _towerSellAllBtn;

    [SerializeField] GameObject _enemyPanel;
    [SerializeField] TextMeshProUGUI _enemyNameTMP;
    [SerializeField] TextMeshProUGUI _enemyLivesTMP;
    [SerializeField] TextMeshProUGUI _enemyArmorTMP;
    [SerializeField] TextMeshProUGUI _enemySpeedTMP;
    [SerializeField] TextMeshProUGUI _enemyBountyTMP;
    [SerializeField] TextMeshProUGUI _enemyBurnTMP;
    [SerializeField] TextMeshProUGUI _enemySpezialTMP;

    [SerializeField] TooltipManager _toolTipManager;

    int _totalUpgradeCost = 0;

    private void Start()
    {
        Signalbus.Subscirbe<RestartGameSignal>(this);
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<RestartGameSignal>(this);
    }

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
                Debug.Log(ex.Message);
            }
        }

        if(_selectedTower != null)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                if(_selectedTowers.Count == 1)
                {
                    UpgradeTower();
                }
                else
                {
                    UpgradeAllTowers();
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (_selectedTowers.Count == 1)
                {
                    SellTower();
                }
                else
                {
                    SellAllTowers();
                }
            }
        }
    }

    public void DisplayTower()
    {
        if (_selectedTower == null) return;

        _towerNameTMP.text = _selectedTower.GetName();

        if (_selectedTower.IsGold())
        {
            _goldTowerTMP.text = $"This tower cannot cause any damage" +
                $"\nIt produces {_selectedTower.GetDmg()} gold" +
                $"\nevery {_selectedTower.GetAttackSpeedValue():F0} seconds";
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
            if(!_selectedTower.IsIce())
            {
                _targetMode.SetActive(true);
                _targetModeDropdown.gameObject.SetActive(true);
                SetTargetOptions();
            }
            else
            {
                _targetMode.SetActive(false);
                _targetModeDropdown.gameObject.SetActive(false);
            }
            _goldTowerTMP.gameObject.SetActive(false);
            _towerValuePanel.SetActive(true);
        }

        if (_selectedTower.GetLvl() >= 4)
        {
            _towerUpgradeBtn.SetActive(false);
        }
        else
        {
            _towerUpgradeCostTMP.text = $"{_selectedTower.GetPrice()} gold";
            _towerUpgradeBtn.SetActive(true);
        }
        _towerRefundValueTMP.text = $"{_selectedTower.GetRefund()} gold";
        _towerSellBtn.SetActive(true);
        _towerSellAllBtn.SetActive(false);
        _towerUpgradeAllBtn.SetActive(false);
        _towerCountValueTMP.gameObject.SetActive(false);
        _towerCount.SetActive(false);
        _towerPanel.SetActive(true);
        _towerCountValueSingleTMP.gameObject.SetActive(false);
    }

    // TODO deselect hide range
    // TODO upgrade all
    // TODO sell all

    public void DisplayTowers()
    {
        if (_selectedTowers.Count <= 0)
        {
            return;
        }

        if (_selectedTowers.Count == 1)
        {
            _selectedTower = _selectedTowers[0];
            DisplayTower();
            return;
        }

        if (AllSameTypeAndLvl())
        {
            _selectedTower = _selectedTowers[0];
            DisplayTower();

            _towerCountValueTMP.text = $"{_selectedTowers.Count}";
            _towerCountValueTMP.gameObject.SetActive(true);
            _towerCount.SetActive(true);

            _totalUpgradeCost = 0;
            int totalRefund = 0;
            foreach (Tower tower in _selectedTowers)
            {
                _totalUpgradeCost += tower.GetPrice();
                totalRefund += tower.GetRefund();
            }

            if (_selectedTower.GetLvl() >= 4)
            {
                _towerUpgradeAllBtn.SetActive(false);
            }
            else
            {
                _towerUpgradeAllCostTMP.text = $"{_totalUpgradeCost} gold";
                _towerUpgradeAllBtn.SetActive(true);
            }

            _towerRefundAllValueTMP.text = $"{totalRefund} gold";

            _towerSellBtn.SetActive(false);
            _towerSellAllBtn.SetActive(true);
            _towerUpgradeBtn.SetActive(false);
            _towerCountValueSingleTMP.gameObject.SetActive(false);
            if (!_selectedTower.IsGold())
            {
                _towerValuePanel.SetActive(true);
            }
            else
            {
                _towerCountValueSingleTMP.text = $"{_selectedTowers.Count} towers selected";
                _towerCountValueSingleTMP.gameObject.SetActive(true);
            }
        }
        else
        {
            _towerNameTMP.text = "Multiple Towers";
            _towerCountValueSingleTMP.text = $"{_selectedTowers.Count} towers selected";
            _towerCountValueSingleTMP.gameObject.SetActive(true);

            _towerUpgradeAllBtn.SetActive(false);

            _totalUpgradeCost = 0;
            int totalRefund = 0;
            foreach (Tower tower in _selectedTowers)
            {
                totalRefund += tower.GetRefund();
                if (tower.GetLvl() < 4)
                {
                    _totalUpgradeCost += tower.GetPrice();
                    _towerUpgradeAllBtn.SetActive(true);
                }
            }

            _towerUpgradeAllCostTMP.text = $"{_totalUpgradeCost} gold";
            _towerRefundAllValueTMP.text = $"{totalRefund} gold";

            _towerSellBtn.SetActive(false);
            _towerSellAllBtn.SetActive(true);
            _towerUpgradeBtn.SetActive(false);
            _towerValuePanel.SetActive(false);
        }
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
        if (_selectedTower.GetLvl() == 4)
        {
            _toolTipManager.HideTooltip();
        }
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

    public void UpgradeAllTowers()
    {
        if (_selectedTowers.Count == 0 || GameController.Instance.GetGold() < _totalUpgradeCost) return;

        bool anyTowerNotMax = false;

        foreach (Tower tower in _selectedTowers)
        {
            if(tower.GetLvl() < 4)
            {
                tower.Upgrade();
            }
            if (tower.GetLvl() < 4)
            {
                anyTowerNotMax = true;
            }
        }

        DisplayTowers();

        if (!anyTowerNotMax)
        {
            _toolTipManager.HideTooltip();
        }
    }

    public void SellAllTowers()
    {
        if (_selectedTowers.Count == 0) return;

        foreach (Tower tower in _selectedTowers)
        {
            tower.Sell(tower.GetName());
        }
        _towerPanel.SetActive(false);
        _selectedTower = null;
        ClearSelection();
        _toolTipManager.HideTooltip();
    }

    public void AddTowerToSelection(Tower selected)
    {
        if (selected == null) return;
        
        if (!_selectedTowers.Contains(selected))
        {
            _selectedTowers.Add(selected);
            selected.SetRangeIndicatorActive(true);
            DisplayTowers();
        }
        else
        {
            _selectedTowers.Remove(selected);
            selected.SetRangeIndicatorActive(false);
            DisplayTowers();
        }
        
    }

    public void Select(GameObject selected)
    {
        ClearSelection();

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
            _selectedTowers.Add(_selectedTower);
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

    public void ClearSelection()
    {
        foreach (Tower tower in _selectedTowers)
        {
            tower.SetRangeIndicatorActive(false);
        }
        _selectedTowers.Clear();
        _towerPanel.SetActive(false);
        _enemyPanel.SetActive(false);
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

            _targetModeDropdown.value = 1;
            _targetModeDropdown.value = _selectedTower.GetTargetMode();

            _targetModeDropdown.onValueChanged.AddListener(SetTargetMode);
        }
    }

    private bool AllSameTypeAndLvl()
    {
        if(_selectedTowers.Count <= 1)
        {
            return true;
        }

        int lvl = _selectedTowers[0].GetLvl();
        bool isArrow = _selectedTowers[0].IsArrow();
        bool isCannon = _selectedTowers[0].IsCannon();
        bool isFire = _selectedTowers[0].IsFire();
        bool isIce = _selectedTowers[0].IsIce();
        bool isGold = _selectedTowers[0].IsGold();

        foreach (Tower tower in _selectedTowers)
        {
            if (tower.GetLvl() != lvl || tower.IsArrow() != isArrow || tower.IsCannon() != isCannon || tower.IsFire() != isFire
                || tower.IsIce() != isIce || tower.IsGold() != isGold)
            {
                return false;
            }
        }

        return true;
    }

    private void SetTargetMode(int targetMode)
    {
        foreach (Tower tower in _selectedTowers)
        {
            tower.SetTargetMode(targetMode);
        }
    }

    public void OnSignalReceived(RestartGameSignal signal)
    {
        _towerPanel.SetActive(false);
        _enemyPanel.SetActive(false);
        _selectedTower = null;
        _selectedEnemy = null;
        ClearSelection();
    }
}
