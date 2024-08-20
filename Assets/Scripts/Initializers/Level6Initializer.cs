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

        Building monument = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[10, 10], _monument, true, true);

        Building.OnBuildingCaptured += HandleHqCaptured;


        Dictionary<int, Building> hqDicts = new Dictionary<int, Building>();
        hqDicts[1] = _hq1;
        hqDicts[2] = _hq2;
        hqDicts[3] = _hq3;

        AIManager ai1 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai1.setPlayers(2);
        ai1.SetHQs(hqDicts);
        ai1.SetGoals(new List<Building>() { monument, _hq1 });

        ai1.AddBuilding(_hq1);
        ai1.AddBuilding(_hq2);
        ai1.AddBuilding(_hq3);
        ai1.AddBuilding(monument);

        AIManager ai2 = Instantiate(LevelManager.Instance.aiManagerPrefab).GetComponent<AIManager>();

        ai2.setPlayers(3);
        ai2.SetHQs(hqDicts);
        ai2.SetGoals(new List<Building>() { monument, _hq1 });

        ai2.AddBuilding(_hq1);
        ai2.AddBuilding(_hq2);
        ai2.AddBuilding(_hq3);
        ai2.AddBuilding(monument);

        ai1.StartPlaying();
        ai2.StartPlaying();
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
