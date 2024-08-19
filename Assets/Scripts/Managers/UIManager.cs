using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _buildMenu;
    [SerializeField] private GameObject _buildWheel;
    [SerializeField] private GameObject _selectedMenu;
    [SerializeField] private GameObject _selectedWheel;
    [SerializeField] private GameObject _upgradeButton;
    [SerializeField] private GameObject _downgradeButton;
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

    public void OpenSelectedMenu(Building building)
    {
        _selectedMenu.SetActive(true);
        _selectedWheel.GetComponent<RectTransform>().position = Input.mousePosition;
        _upgradeButton.GetComponent<UpgradeButton>().buildingInformation = building.BuildingInformation;
        _downgradeButton.GetComponent<DowngradeButton>().buildingInformation = building.BuildingInformation;
        LevelManager.Instance.Selected = building.Cell;
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
        _selectedMenu.SetActive(false);
    }

    public void UpdateCurrencyText()
    {
        _currencyText.text = $"$ {LevelManager.Instance.Currencies[1]:00.00}";
    }
}
