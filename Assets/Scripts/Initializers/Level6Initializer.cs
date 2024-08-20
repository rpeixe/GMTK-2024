using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level6Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private BuildingInformation _hq10;
    [SerializeField] private BuildingInformation _monument;

    private Building _hq1;
    private Building _hq2;
    private Building _hq3;

    public void InitializeLevel()
    {
        _hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[13,1], _hq, true, true);
        _hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[17,17], _hq, true, true);
        _hq3 = LevelManager.Instance.ConstructBuilding(3, LevelManager.Instance.GridController.Cells[1,13], _hq, true, true);

        LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[10, 10], _monument, true, true);

        Building.OnBuildingCaptured += HandleHqCaptured;
    }

    private void HandleHqCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type != BuildingInformation.BuildingType.hq) return;

        if (_hq1.Owner != 1 && _hq2.Owner != 1 && _hq3.Owner != 1)
        {
            LevelManager.Instance.Defeat();
        }
    }

    private void HandleHqUpgraded(Building building, int owner)
    {
        if (building.BuildingInformation == _hq10)
        {
            if (owner == 1)
            {
                LevelManager.Instance.Victory();
            }
            else
            {
                LevelManager.Instance.Defeat();
            }
        }
    }
}
