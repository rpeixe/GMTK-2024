using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DowngradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI _priceText;
    private Button _button;
    private BuildingInformation _buildingInformation;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void Update()
    { 
        UpdateCosts();
        
    }

    private void UpdateCosts()
    {
        _buildingInformation = LevelManager.Instance.Selected.ConstructedBuilding.BuildingInformation;

        if (_buildingInformation.Previous == null)
        {
            _button.interactable = false;
            _priceText.text = $"";
            return;
        }

        float cost = LevelManager.Instance.CalculateCost(1, LevelManager.Instance.Selected, _buildingInformation.Previous)/2;
        _priceText.text = $"$ {cost:00.00}";

        if (LevelManager.Instance.Selected.ConstructedBuilding.Deactivated)
        {
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
        }
    }

    private void HandleClick()
    {
        LevelManager.Instance.DowngradeBuilding(LevelManager.Instance.Selected);
        UIManager.Instance.Unselect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_buildingInformation.Previous)
        {
            UIManager.Instance.ShowBuildingInfo(_buildingInformation.Previous);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideBuildingInfo();
    }
}
