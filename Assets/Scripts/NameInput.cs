using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] Button _button;
    [SerializeField] DataManager _dataManager;
    [SerializeField] GameObject _panel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && IsInputValid(_inputField.text))
        {
            _dataManager.SetPlayerName(_inputField.text);
            _panel.SetActive(false);
        }
    }

    void Start()
    {
        _inputField.onValueChanged.AddListener(OnInputValueChanged);

        _button.interactable = false;

        _button.onClick.AddListener(OnButtonClick);
    }

    void OnInputValueChanged(string newValue)
    {
        bool isValid = IsInputValid(newValue);

        _button.interactable = isValid;
    }

    bool IsInputValid(string input)
    {
        if (input.Trim().Length < 3)
        {
            return false;
        }

        if (input.Length > 32)
        {
            return false;
        }

        foreach (char c in input)
        {
            if (!(char.IsLetterOrDigit(c) || c == ' '))
            {
                return false;
            }
        }

        bool hasNonDigit = false;
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
            {
                hasNonDigit = true;
                break;
            }
        }

        return hasNonDigit;
    }

    void OnButtonClick()
    {
        _dataManager.SetPlayerName(_inputField.text);
        _panel.SetActive(false);
    }
}
