using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int Owner { get; set; } = 0;
    public GridCell Cell { get; set; }
    public BuildingInformation BuildingInformation { get; set; }
    public bool Deactivated { get; set; } = false;
    public Dictionary<int, float> Damage { get; set; } = new Dictionary<int, float>();
    public float MarketingSpeed { get; set; } = 1f;
    
    public Building Target { get; set; } = null;

    private float marketing;

    private float _cooldownTime = 0.2f;
 
    private bool _onCooldown = false;
    public bool InRange(Building Target)
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
                if (Target.Cell.Position == new Vector2Int(Cell.Position.x + x, Cell.Position.y + y))
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

                var xPos = Mathf.Clamp(Cell.Position.x + x, 0, LevelManager.Instance.MapWidth);
                var yPos = Mathf.Clamp(Cell.Position.y + y, 0, LevelManager.Instance.MapHeight);
                Building target = cells[xPos, yPos].ConstructedBuilding;
                Debug.Log($"{cells}");
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

                var xPos = Mathf.Clamp(Cell.Position.x + x,0,LevelManager.Instance.MapWidth);
                var yPos = Mathf.Clamp(Cell.Position.y + y,0,LevelManager.Instance.MapHeight);
                
                if (boolean)
                {
                    cells[xPos, yPos].Buildable[Owner]++;

                    if (Owner == 0 && cells[xPos, yPos].CellType == GridCell.CellTypes.Buildable)
                    {
                        LevelManager.Instance.GridController.SetGroundTileColor(new Vector2Int(xPos, yPos), Color.gray);
                    }

                    else if (Owner == 1 && cells[xPos,yPos].CellType == GridCell.CellTypes.Buildable)
                    {
                        LevelManager.Instance.GridController.SetGroundTileColor(new Vector2Int(xPos,yPos), Color.red);
                    }

                    else if (Owner == 2 && cells[xPos, yPos].CellType == GridCell.CellTypes.Buildable)
                    {
                        LevelManager.Instance.GridController.SetGroundTileColor(new Vector2Int(xPos, yPos), Color.green);
                    }

                    else if (Owner == 3 && cells[xPos, yPos].CellType == GridCell.CellTypes.Buildable)
                    {
                        LevelManager.Instance.GridController.SetGroundTileColor(new Vector2Int(xPos, yPos), Color.blue);
                    }
                }

                else
                {
                    cells[xPos, yPos].Buildable[Owner]--;
                    if (cells[xPos, yPos].Buildable[Owner] == 0)
                    {
                        LevelManager.Instance.GridController.SetGroundTileColor(new Vector2Int(xPos, yPos), Color.white);
                    }
                }
            }
        }
    }
    public Building GetFirstTarget()
    {
        var cells = LevelManager.Instance.GridController.Cells;
        int radius = BuildingInformation.InfluenceRadius;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > radius)
                {
                    continue;
                }

                var xPos = Mathf.Clamp(Cell.Position.x + x, 0, LevelManager.Instance.MapWidth);
                var yPos = Mathf.Clamp(Cell.Position.y + y, 0, LevelManager.Instance.MapHeight);
                Building target = cells[xPos, yPos].ConstructedBuilding;

                if (cells[xPos, yPos].ConstructedBuilding?.Owner != Owner &&
                    cells[xPos, yPos].ConstructedBuilding?.Owner != null)
                {
                    LevelManager.Instance.GridController.SetTileColor(Cell.Position, Color.cyan);
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
        gameObject.AddComponent<GenerateIncome>().Init(this);
        BuildingInformation = buildingInformation;
        _cooldownTime = 1 / MarketingSpeed;
        marketing = BuildingInformation.InfluenceValue;
        LevelManager.Instance.GridController.SetBuilding(cell, this);
        for (int i = 0; i <= LevelManager.Instance.NumPlayers; i++)
        {
            Damage[i] = 0;
        }
        if (!instant)
        {
            Deactivate();
            Invoke(nameof(HandleBuildComplete), buildingInformation.BuildingTime);
        }
        if (buildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(true);
        }
    }

    public void ChangeOwner(int player)
    {
        for (int i = 1; i <= LevelManager.Instance.NumPlayers; i++)
        {
            Damage[i] = 0;
        }
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(false);
            Owner = player;
            ToggleBuilding(true);
            return;
        }
        
        Owner = player;
        
    }

    public IEnumerator BuildingCooldown()
    {
        _onCooldown = true;
        Debug.Log("cooldown");
        yield return new WaitForSeconds(_cooldownTime);
        _onCooldown = false;

    }

    public void Capture(Building target)
    {
        target.Damage[Owner] += marketing;
        if (target.Damage[Owner] == target.BuildingInformation.BaseCost)
        {
            target.ChangeOwner(Owner);
        }
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
        Build(Owner, Cell, BuildingInformation.Evolution);
    }

    public void Downgrade()
    {
        Build(Owner, Cell, BuildingInformation.Previous);
    }

    public void ProcessTick()
    {

    }

    public void Update()
    {
        if (Owner == 0)
        {
            
        }

        else if (Target == null)
        {
            LevelManager.Instance.GridController.SetTileColor(Cell.Position, Color.white);
            Target = GetFirstTarget();
        }

        else if (!_onCooldown)
        {
            Capture(Target);
            StartCoroutine(BuildingCooldown());
        }
    }

}
