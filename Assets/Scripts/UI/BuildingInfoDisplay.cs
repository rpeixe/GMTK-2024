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

    private RectTransform _rectTransform;

    public BuildingInformation Information { get; set; }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _nameText.text = Information.name;
        _descriptionText.text = Information.Description;
        _priceText.text = $"{LevelManager.Instance.CalculateCost(1, LevelManager.Instance.Selected, Information):00.00}";
        _incomeText.text = $"{Information.Income * LevelManager.Instance.Selected.RegionClass.ResourceGenerationFactor:00.00} / s";
    }

    private void Update()
    {
        if (Input.mousePosition.x < Screen.width)
        {
            _rectTransform.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            _rectTransform.pivot = new Vector2(0f, 0.5f);
        }
        _rectTransform.position = Input.mousePosition;
    }
}
