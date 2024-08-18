using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeText;
    public BuildingInformation buildingInformation;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void Update()
    { 
        if (_button.IsActive())
        {
            UpdateCosts();
        }
        
    }

    private void UpdateCosts()
    {
        if (buildingInformation.Evolution == null)
        {
            _button.interactable = false;
            upgradeText.text = $"Upgrade\nCost: n/a";
            return;
        }

        float cost = LevelManager.Instance.CalculateCost(1, LevelManager.Instance.Selected, buildingInformation.Evolution);
        upgradeText.text = $"Upgrade\nCost: {cost}";

        if (LevelManager.Instance.Currencies[1] < cost || LevelManager.Instance.Selected.ConstructedBuilding.Deactivated)
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
        LevelManager.Instance.UpgradeBuilding(LevelManager.Instance.Selected);
        UIManager.Instance.Unselect();
    }
}
