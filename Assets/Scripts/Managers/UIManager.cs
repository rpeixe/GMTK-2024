using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _buildMenu;
    [SerializeField] private GameObject _buildWheel;
    [SerializeField] private TextMeshProUGUI _currencyText;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateCurrencyText), 0f, 0.01f);
    }

    public void OpenBuildMenu(GridCell cell)
    {
        _buildMenu.SetActive(true);
        _buildWheel.GetComponent<RectTransform>().position = Input.mousePosition;
        LevelManager.Instance.Selected = cell;
    }

    public void Unselect()
    {
        LevelManager.Instance.Selected = null;
        _buildMenu.SetActive(false);
    }

    public void UpdateCurrencyText()
    {
        _currencyText.text = $"$ {LevelManager.Instance.Currencies[1]:00.00}";
    }
}
