using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int Owner { get; set; } = 0;
    public GridCell Cell { get; set; }
    public BuildingInformation BuildingInformation { get; set; }
    public bool Deactivated { get; set; } = false;

    public void Build(int player, GridCell cell, BuildingInformation buildingInformation)
    {
        Owner = player;
        Cell = cell;
        BuildingInformation = buildingInformation;
        gameObject.AddComponent<GenerateIncome>().Init(this);
        LevelManager.Instance.GridController.SetBuilding(cell, this);
        Deactivate();
        Invoke(nameof(HandleBuildComplete), buildingInformation.BuildingTime);
    }

    public void HandleBuildComplete()
    {
        Activate();
    }

    public void Deactivate()
    {
        Deactivated = true;
        LevelManager.Instance.GridController.SetTileColor(Cell.Position, new Color(0.3f, 0.3f, 0.3f));
    }

    public void Activate()
    {
        Deactivated = false;
        LevelManager.Instance.GridController.SetTileColor(Cell.Position, new Color(1f, 1f, 1f));
    }

    public void Sell()
    {
        LevelManager.Instance.AddCurrency(Owner, BuildingInformation.BaseCost / 10);
        LevelManager.Instance.GridController.SetBuilding(Cell, null);
        Destroy(gameObject);
    }

    public void Upgrade()
    {

    }

    public void ProcessTick()
    {

    }
}
