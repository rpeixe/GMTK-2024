using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level7Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;
    [SerializeField] private BuildingInformation _hq3;
    [SerializeField] private BuildingInformation _monument;

    private int _playerMonuments = 0;

    public void InitializeLevel()
    {
        Building hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[17,18], _hq, true, true);
        Building hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[7,7], _hq3, true, true);

        Building monument1 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[11, 11], _monument, true, true);
        Building monument2 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[12, 7], _monument, true, true);
        Building monument3 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[11, 2], _monument, true, true);
        Building monument4 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[6, 1], _monument, true, true);
        Building monument5 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[1, 1], _monument, true, true);
        Building monument6 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[1, 7], _monument, true, true);
        Building monument7 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[2, 12], _monument, true, true);
        Building monument8 = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[7, 13], _monument, true, true);

        hq1.OnBuildingCaptured += HandleHqCaptured;
        hq2.OnBuildingCaptured += HandleHqCaptured;

        monument1.OnBuildingCaptured += HandleMonumentCaptured;
        monument2.OnBuildingCaptured += HandleMonumentCaptured;
    }

    private void HandleMonumentCaptured(Building building, int oldOwner, int newOwner)
    {
        if (newOwner == 1)
        {
            _playerMonuments++;
        }
        if (oldOwner == 1)
        {
            _playerMonuments--;
        }
        if (_playerMonuments == 8)
        {
            LevelManager.Instance.Victory();
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
