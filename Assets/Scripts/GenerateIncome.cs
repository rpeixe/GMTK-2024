using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateIncome : MonoBehaviour
{
    private Building _parentBuilding;

    private void Update()
    {
        if (_parentBuilding != null && !_parentBuilding.Deactivated)
        {
            AddIncome();
        }
    }

    public void Init(Building parentBuilding)
    {
        _parentBuilding = parentBuilding;
    }

    public void AddIncome()
    {
        float income = _parentBuilding.BuildingInformation.Income * _parentBuilding.Cell.RegionClass.ResourceGenerationFactor;
        if (income > 0)
        {
            LevelManager.Instance.AddCurrency(_parentBuilding.Owner, income * Time.deltaTime);
        }
        else if (income < 0)
        {
            LevelManager.Instance.RemoveCurrency(_parentBuilding.Owner, -income * Time.deltaTime);
        }
    }
}
