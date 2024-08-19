using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _incomeText;
    [SerializeField] private GameObject _infoPanel;

    private RectTransform _infoPanelRectTransform;
    private BuildingInformation _buildingInformation;

    public void Popup(BuildingInformation buildingInformation, GridCell cell)
    {
        _buildingInformation = buildingInformation;
        _nameText.text = buildingInformation.name;
        _descriptionText.text = buildingInformation.Description;
        _priceText.text = $"{LevelManager.Instance.CalculateCost(1, cell, buildingInformation):00.00}";
        _incomeText.text = $"{buildingInformation.Income * cell.RegionClass.ResourceGenerationFactor:00.00} / s";
        _infoPanel.SetActive(true);
    }

    public void Hide()
    {
        _infoPanel.SetActive(false);
    }

    private void Start()
    {
        _infoPanelRectTransform = _infoPanel.GetComponent<RectTransform>();
    }

    private void Update()
    {
        CalculatePosition();
    }

    private void CalculatePosition()
    {
        if (Input.mousePosition.x > Screen.width/2)
        {
            _infoPanelRectTransform.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            _infoPanelRectTransform.pivot = new Vector2(0f, 0.5f);
        }
        _infoPanelRectTransform.position = Input.mousePosition;
    }
}
