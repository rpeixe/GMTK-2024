using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DowngradeButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _priceText;
    private Button _button;

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
        BuildingInformation buildingInformation = LevelManager.Instance.Selected.ConstructedBuilding.BuildingInformation;

        if (buildingInformation.Previous == null)
        {
            _button.interactable = false;
            _priceText.text = $"";
            return;
        }

        float cost = LevelManager.Instance.CalculateCost(1, LevelManager.Instance.Selected, buildingInformation.Previous)/2;
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
}
