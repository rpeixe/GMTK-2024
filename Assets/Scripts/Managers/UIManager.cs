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
    [SerializeField] private BuildingInfoDisplay _buildingInfoDisplay;
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
        HideBuildingInfo();
    }

    public void ShowBuildingInfo(BuildingInformation buildingInformation, GridCell cell = null)
    {
        _buildingInfoDisplay.Popup(buildingInformation, cell == null ? LevelManager.Instance.Selected : cell);
    }

    public void HideBuildingInfo()
    {
        _buildingInfoDisplay.Hide();
    }

    public void UpdateCurrencyText()
    {
        _currencyText.text = LevelManager.Instance.Currencies[1].ToString("00.00");
    }
}
