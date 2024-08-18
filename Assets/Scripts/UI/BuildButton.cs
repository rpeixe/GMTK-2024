using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] BuildingInformation buildingInformation;
    [SerializeField] TextMeshProUGUI priceText;
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
        float cost = LevelManager.Instance.CalculateCost(1, LevelManager.Instance.Selected, buildingInformation);
        priceText.text = $"$ {cost:00.00}";
        if (LevelManager.Instance.Currencies[1] < cost)
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
        LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.Selected, buildingInformation);
        UIManager.Instance.Unselect();
    }
}
