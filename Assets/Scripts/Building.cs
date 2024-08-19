using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Building : MonoBehaviour
{
    public int Owner { get; set; } = 0;
    public GridCell Cell { get; set; }
    public BuildingInformation BuildingInformation { get; set; }
    public bool Deactivated { get; set; } = false;
    public bool inRange(Building Target)
    {
        int radius = BuildingInformation.InfluenceRadius;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > radius)
                {
                    continue;
                }
                if(Target.Cell.Position == new Vector2Int(Cell.Position.x + x, Cell.Position.y + y))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public ArrayList GetTargets()
    {
        var cells = LevelManager.Instance.GridController.Cells;
        var targetList = new System.Collections.ArrayList();
        int radius = BuildingInformation.InfluenceRadius;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > radius)
                {
                    continue;
                }

                var xPos = Mathf.Clamp(Cell.Position.x + x, 0, 19);
                var yPos = Mathf.Clamp(Cell.Position.y + y, 0, 9);
                Building target = cells[xPos, yPos].ConstructedBuilding;

                if (cells[xPos,yPos].ConstructedBuilding?.Owner != Owner)
                {
                    targetList.Add(target);
                }
            }
        }
        return targetList;
    }

    public void ToggleBuilding(bool boolean)
    {
        int radius = BuildingInformation.InfluenceRadius;
        var cells = LevelManager.Instance.GridController.Cells;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > radius)
                {
                    continue;
                }

                var xPos = Mathf.Clamp(Cell.Position.x + x,0,19);
                var yPos = Mathf.Clamp(Cell.Position.y + y,0,9);

                cells[xPos, yPos].Buildable[Owner] = boolean;
                if (boolean && Owner==1)
                {
                    LevelManager.Instance.GridController.SetGroundTileColor(new Vector2Int(xPos,yPos), Color.red);
                }
                
                else
                {
                    LevelManager.Instance.GridController.SetGroundTileColor(new Vector2Int(xPos, yPos), Color.white);
                }
            }
        }
    }
    public Building GetFirstTarget()
    {
        var cells = LevelManager.Instance.GridController.Cells;
        var targetList = new System.Collections.ArrayList();
        int radius = BuildingInformation.InfluenceRadius;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > radius)
                {
                    continue;
                }

                var xPos = Cell.Position.x + x;
                var yPos = Cell.Position.y + y;
                Building target = cells[xPos, yPos].ConstructedBuilding;

                if (cells[xPos, yPos].ConstructedBuilding?.Owner != Owner)
                {
                    return target;
                }
            }
        }
        return null;
    }

    public void Build(int player, GridCell cell, BuildingInformation buildingInformation, bool instant = false)
    {
        Owner = player;
        Cell = cell;
        BuildingInformation = buildingInformation;
        gameObject.AddComponent<GenerateIncome>().Init(this);
        LevelManager.Instance.GridController.SetBuilding(cell, this);
        if (!instant)
        {
            Deactivate();
            Invoke(nameof(HandleBuildComplete), buildingInformation.BuildingTime);
        }

        else
        {
            ToggleBuilding(BuildingInformation.PermitsBuildingWithinRange);
        }
    }

    public void Attack(Building target)
    {

    }

    public Building TargetBuilding(Building target)
    {
        if (inRange(target))
        {
            Attack(target);
            return target;
        }
        return null;
    }

    public void HandleBuildComplete()
    {
        Activate();
        
    }

    public void Deactivate()
    {
        Deactivated = true;
        LevelManager.Instance.GridController.SetTileColor(Cell.Position, new Color(0.3f, 0.3f, 0.3f));
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(false);
        }
    }

    public void Activate()
    {
        Deactivated = false;
        LevelManager.Instance.GridController.SetTileColor(Cell.Position, new Color(1f, 1f, 1f));
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(true);
        }
    }

    public void Sell()
    {
        LevelManager.Instance.AddCurrency(Owner, BuildingInformation.BaseCost / 10);
        LevelManager.Instance.GridController.SetBuilding(Cell, null);
        Destroy(gameObject);
    }

    public void Upgrade()
    {
        Build(this.Owner, this.Cell, this.BuildingInformation.Evolution);
    }

    public void Downgrade()
    {
        Build(this.Owner, this.Cell, this.BuildingInformation.Previous);
    }

    public void ProcessTick()
    {

    }

    public void OnCapture()
    {
        ToggleBuilding(false);
        ToggleBuilding(true);
    }
}
