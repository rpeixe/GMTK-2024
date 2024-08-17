using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] BuildingInformation buildingInformation;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.Selected, buildingInformation);
    }
}
