using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BuildingInformation _buildingInformation;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private BuildingInfoDisplay _buildingInfoDisplay;
    [SerializeField] private AudioClip _buildSound;
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
        float cost = LevelManager.Instance.CalculateCost(1, LevelManager.Instance.Selected, _buildingInformation);
        _priceText.text = $"$ {cost:00.00}";
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
        LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.Selected, _buildingInformation);
        AudioManager.Instance.PlayEffect(_buildSound);
        UIManager.Instance.Unselect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowBuildingInfo(_buildingInformation);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideBuildingInfo();
    }
}
