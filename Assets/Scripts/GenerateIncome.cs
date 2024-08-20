using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateIncome : MonoBehaviour
{
    private Building _parentBuilding;
    private bool _applyingIncome;

    public void Init(Building parentBuilding)
    {
        _parentBuilding = parentBuilding;
    }

    public void ToggleIncome(bool boolean)
    {
        if (_applyingIncome == boolean)
        {
            return;
        }
        _applyingIncome = boolean;

        float income = _parentBuilding.BuildingInformation.Income * _parentBuilding.Cell.RegionClass.ResourceGenerationFactor;
        if (boolean)
        {
            LevelManager.Instance.Incomes[_parentBuilding.Owner] += income;
        }
        else
        {
            LevelManager.Instance.Incomes[_parentBuilding.Owner] -= income;
        }
    }
}
