using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level4Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] private BuildingInformation _hq;

    public void InitializeLevel()
    {
        Building hq1 = LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[14,4], _hq, true, true);
        Building hq2 = LevelManager.Instance.ConstructBuilding(2, LevelManager.Instance.GridController.Cells[5,14], _hq, true, true);


        Dictionary<int, Building> hqDicts = new Dictionary<int, Building>();
        hqDicts[1] = hq1;
        hqDicts[2] = hq2;

        Building.OnBuildingCaptured += HandleHqCaptured;

        AIManager ai1 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai1.setPlayers(2);
        ai1.SetHQs(hqDicts);
        ai1.SetGoals(new List<Building>() { hq1 });

        ai1.AddBuilding(hq1);
        ai1.AddBuilding(hq2);

        ai1.StartPlaying();
    }

    private void HandleHqCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type != BuildingInformation.BuildingType.hq) return;

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
