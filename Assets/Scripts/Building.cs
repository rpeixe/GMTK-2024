using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int Owner { get; set; } = 0;
    public GridCell Cell { get; set; }

    public BuildingInformation BuildingInformation { get; set; }

    public void Build(int player, GridCell cell, BuildingInformation buildingInformation)
    {
        Owner = player;
        Cell = cell;
        BuildingInformation = buildingInformation;
        LevelManager.Instance.GridController.SetBuilding(cell, this);
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
