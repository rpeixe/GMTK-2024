using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _gui;
    [SerializeField] private GameObject _buildMenu;
    [SerializeField] private GameObject _buildWheel;
    [SerializeField] private GameObject _selectedMenu;
    [SerializeField] private GameObject _selectedWheel;
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _defeatScreen;
    [SerializeField] private BuildingInfoDisplay _buildingInfoDisplay;
    [SerializeField] private TextMeshProUGUI _currencyText;
    [SerializeField] private TextMeshProUGUI _incomeText;
    [SerializeField] private GameObject _bankruptcyText;

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
        float x = Mathf.Clamp(Input.mousePosition.x, (1f / 5) * Screen.width, (4f / 5) * Screen.width);
        float y = Mathf.Clamp(Input.mousePosition.y, (1f / 5) * Screen.width, (4f / 5) * Screen.height);
        _selectedWheel.GetComponent<RectTransform>().position = new Vector2(x, y);
        LevelManager.Instance.Selected = building.Cell;
    }


    public void OpenBuildMenu(GridCell cell)
    {
        _buildMenu.SetActive(true);
        float x = Mathf.Clamp(Input.mousePosition.x, (1f / 5) * Screen.width, (4f / 5) * Screen.width);
        float y = Mathf.Clamp(Input.mousePosition.y, (1f / 5) * Screen.width, (4f / 5) * Screen.height);
        _buildWheel.GetComponent<RectTransform>().position = new Vector2(x, y);
        LevelManager.Instance.Selected = cell;
    }

    public void Unselect()
    {
        LevelManager.Instance.Selected = null;
        _buildMenu.SetActive(false);
        _selectedMenu.SetActive(false);
        HideBuildingInfo();
    }

    public void ShowGui()
    {
        _gui.SetActive(true);
    }

    public void HideGui()
    {
        _gui.SetActive(false);
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
        _incomeText.text = LevelManager.Instance.PlayerIncome.ToString("00.00");
    }

    public void ShowVictoryScreen()
    {
        Unselect();
        _victoryScreen.SetActive(true);
    }

    public void ShowDefeatScreen()
    {
        Unselect();
        _defeatScreen.SetActive(true);
    }

    public void ShowBankruptcyTimer()
    {
        _bankruptcyText.SetActive(true);
    }

    public void HideBankruptcyTimer()
    {
        _bankruptcyText.SetActive(false);
    }
}
