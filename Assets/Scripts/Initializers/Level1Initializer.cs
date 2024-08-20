using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Initializer : MonoBehaviour, ILevelInitializer
{
    [SerializeField] BuildingInformation _hq;
    [SerializeField] BuildingInformation _monument;

    public void InitializeLevel()
    {
        Building monument = LevelManager.Instance.ConstructBuilding(0, LevelManager.Instance.GridController.Cells[6,5], _monument, true, true);
        LevelManager.Instance.ConstructBuilding(1, LevelManager.Instance.GridController.Cells[12,3], _hq, true, true);

        Building.OnBuildingCaptured += HandleMonumentCaptured;
    }

    private void HandleMonumentCaptured(Building building, int oldOwner, int newOwner)
    {
        if (building.BuildingInformation.Type != BuildingInformation.BuildingType.monument) return;

        LevelManager.Instance.Victory();
    }
}
