using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] Shockwave _shockwave;
    [SerializeField] GameObject _tooltipPrefab;

    public static TooltipManager Instance;

    GameObject _tooltipInstance;
    TextMeshProUGUI _tooltipText;

    Dictionary<string, string> _tooltipsDict = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _tooltipInstance = Instantiate(_tooltipPrefab, transform);
        _tooltipText = _tooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
        _tooltipInstance.SetActive(false);
        AddTooltips();
    }

    private void Update()
    {
        if (_tooltipInstance.activeSelf)
        {
            Vector2 position = Input.mousePosition;
            if(position.y > Screen.height / 13 * 12)
            {
                position.y -= _tooltipInstance.GetComponent<RectTransform>().rect.height / 2 + 25f;
            }
            if (position.x > Screen.width / 2)
            {
                position.x -= _tooltipInstance.GetComponent<RectTransform>().rect.width / 2 + 25f;
            }
            else
            {
                position.x += _tooltipInstance.GetComponent<RectTransform>().rect.width / 2 + 25f;
            }

            _tooltipInstance.transform.position = position;
        }
    }

    public void ShowTooltip(string tooltipID)
    {
        if (_tooltipsDict.TryGetValue(tooltipID, out string tooltip))
        {
            _tooltipText.text = tooltip;
            _tooltipInstance.SetActive(true);

            StartCoroutine(UpdateTMP());
        }
    }

    public void HideTooltip()
    {
        _tooltipInstance.SetActive(false);
    }

    private void AddTooltips()
    {
        _tooltipsDict.Add("arrow", "A cheap early game tower\nwith rapid single target fire.\nHotkey: \"1\"");
        _tooltipsDict.Add("cannon", "A tower with long range,\nhigh damage and slow\nsingle target fire.\nHotkey: \"2\"");
        _tooltipsDict.Add("fire", "An expensiv tower that\n spreads low damage over time\n effect that ignors armor.\nHotkey: \"3\"");
        _tooltipsDict.Add("ice", "An expensiv tower that\n slows all enemies in range\n and deals low damage over time.\nHotkey: \"4\"");
        _tooltipsDict.Add("goldTower", "A very expensive tower\n that dont attack but\n generates gold over time.\nHotkey: \"5\"");
        _tooltipsDict.Add("boom", $"Kill all enemies!\nMore expensive with each use.\nHotkey: \"Space\"");
        _tooltipsDict.Add("gold", "Gain 1 gold each second.");
        _tooltipsDict.Add("pause", "Pause Game.\nHotkey: \"F1\"");
        _tooltipsDict.Add("play", "Normal speed.\nHotkey: \"F2\"");
        _tooltipsDict.Add("fast", "Double the speed.\nHotkey: \"F3\"");
        _tooltipsDict.Add("faster", "Four times the speed.\nHotkey: \"F4\"");
        _tooltipsDict.Add("upgrade", "Upgrade tower.\nHotkey: \"U\"");
        _tooltipsDict.Add("sell", "Sell tower.\nHotkey: \"S\"");
        _tooltipsDict.Add("upgradeAll", "Upgrade all selected towers.\nHotkey: \"U\"");
        _tooltipsDict.Add("sellAll", "Sell all selected towers.\nHotkey: \"S\"");
    }

    private IEnumerator UpdateTMP()
    {
        yield return new WaitForEndOfFrame();
        RectTransform panelRectTransform = _tooltipInstance.GetComponent<RectTransform>();

        _tooltipText.ForceMeshUpdate();
        RectTransform textRectTransform = _tooltipText.GetComponent<RectTransform>();
        Vector2 textSize = new Vector2(_tooltipText.preferredWidth + 40f, _tooltipText.preferredHeight + 40f);
        textRectTransform.sizeDelta = textSize;

        panelRectTransform.sizeDelta = textRectTransform.sizeDelta;

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRectTransform);
    }
}
