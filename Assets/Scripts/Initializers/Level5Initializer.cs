using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level5Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private BuildingInformation _monument;

    private Building _monument1;
    private Building _monument2;

    public void InitializeLevel()
    {
        Building hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[16,4], _hq, true, true);
        Building hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[4,15], _hq, true, true);

        _monument1 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[2, 7], _monument, true, true);
        _monument2 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[12, 17], _monument, true, true);

        hq1.OnBuildingCaptured += HandleHqCaptured;
        hq2.OnBuildingCaptured += HandleHqCaptured;

        _monument1.OnBuildingCaptured += HandleMonumentCaptured;
        _monument2.OnBuildingCaptured += HandleMonumentCaptured;
    }

    private void HandleMonumentCaptured(Building building, int oldOwner, int newOwner)
    {
        if (_monument1.Owner == 1 && _monument2.Owner == 1)
        {
            LevelManager.Instance.Victory();
        }
        else if (_monument1.Owner == 2 && _monument2.Owner == 2)
        {
            LevelManager.Instance.Defeat();
        }
    }

    private void HandleHqCaptured(Building building, int oldOwner, int newOwner)
    {
        if (oldOwner == 1)
        {
            LevelManager.Instance.Defeat();
        }
        else if (oldOwner == 2)
        {
            LevelManager.Instance.Victory();
        }
    }
}
