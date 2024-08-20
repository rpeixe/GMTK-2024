using System;
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
    public List<Building> Targets { get; set; } = new List<Building>();
    public float BaseCost { get; set; }
    public float Marketing { get; set; }
    public int Radius { get; set; }
    public float Income { get; set; }
    protected bool _searchingTarget = true;
    protected bool _onCooldown = false;
    protected float _searchTargetTick = 0.1f;
    protected float _captureTick = 0.2f;
    protected GenerateIncome _generateIncome;
    protected bool _rangeActive = false;


    public bool IsAllied(Building target)
    {
        return Owner == target.Owner;
    }

    public bool InRange(Building target)
    {
        for (int x = -Radius; x <= Radius; x++)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > Radius)
                {
                    continue;
                }
                if (target.Cell.Position == new Vector2Int(Math.Clamp(Cell.Position.x + x,0,LevelManager.Instance.MapWidth-1), 
                                                           Math.Clamp(Cell.Position.y + y,0, LevelManager.Instance.MapHeight-1)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<Building> GetTargets(bool isAllied)
    {
        
        var cells = LevelManager.Instance.GridController.Cells;
        var targetList = new List<Building>();
        for (int x = -Radius; x <= Radius; x++)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > Radius)
                {
                    continue;
                }

                var xPos = Mathf.Clamp(Cell.Position.x + x, 0, LevelManager.Instance.MapWidth-1);
                var yPos = Mathf.Clamp(Cell.Position.y + y, 0, LevelManager.Instance.MapHeight-1);
                Building target = cells[xPos, yPos].ConstructedBuilding;
                if (cells[xPos, yPos].ConstructedBuilding == null || target == this)
                {
                    continue;
                }
                if (IsAllied(cells[xPos, yPos].ConstructedBuilding) == isAllied)
                {
                    targetList.Add(target);
                }
            }
        }
        return targetList;
    }

    public void ToggleBuilding(bool boolean)
    {
        if (_rangeActive == boolean)
        {
            return;
        }

        _rangeActive = boolean;

        var cells = LevelManager.Instance.GridController.Cells;
        // range logic
        for (int x = -Radius; x <= Radius; x++)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > Radius)
                {
                    continue;
                }

                var xPos = Mathf.Clamp(Cell.Position.x + x,0, LevelManager.Instance.MapWidth-1);
                var yPos = Mathf.Clamp(Cell.Position.y + y,0, LevelManager.Instance.MapHeight-1);

                if (xPos >= LevelManager.Instance.MapWidth || yPos >= LevelManager.Instance.MapHeight
                    || xPos < 0 || yPos < 0)
                {
                    continue;
                }

                if (boolean)
                {
                    cells[xPos, yPos].Buildable[Owner]++;

                    if (Owner == 1 && cells[xPos,yPos].CellType == GridCell.CellTypes.Buildable)
                    {
                        LevelManager.Instance.GridController.SetRangeTile(new Vector2Int(xPos, yPos), true);
                    }

                    else if (Owner == 2 && cells[xPos, yPos].CellType == GridCell.CellTypes.Buildable)
                    {
                        LevelManager.Instance.GridController.SetRangeTile(new Vector2Int(xPos, yPos), false);
                    }

                    else if (Owner == 3 && cells[xPos, yPos].CellType == GridCell.CellTypes.Buildable)
                    {
                        LevelManager.Instance.GridController.SetRangeTile(new Vector2Int(xPos, yPos), false);
                    }
                }

                else
                {
                    cells[xPos,yPos].Buildable[Owner]--;
                    
                    if (Owner == 1 && cells[xPos, yPos].CellType == GridCell.CellTypes.Buildable && cells[xPos, yPos].Buildable[1] <= 0)
                    {
                        LevelManager.Instance.GridController.SetRangeTile(new Vector2Int(xPos, yPos), false);
                    }
                }
            }
        }
    }
    public Building GetFirstTarget()
    {
        var cells = LevelManager.Instance.GridController.Cells;
        for (int x = -Radius; x <= Radius; x++)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > Radius)
                {
                    continue;
                }

                var xPos = Cell.Position.x + x;
                var yPos = Cell.Position.y + y;

                if (xPos >= LevelManager.Instance.MapWidth || yPos >= LevelManager.Instance.MapHeight
                    || xPos < 0 || yPos < 0)
                {
                    continue;
                }
                Building target = cells[xPos, yPos].ConstructedBuilding;

                if (cells[xPos, yPos].ConstructedBuilding?.Owner != Owner &&
                    cells[xPos, yPos].ConstructedBuilding?.Owner != null)
                {
                    return target;
                }
            }
        }
        return null;
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

    public IEnumerator SearchTargetsTick()
    {
        _searchingTarget = false;
        yield return new WaitForSeconds(_searchTargetTick);
        _searchingTarget = true;
    }

    public IEnumerator CaptureTick()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_captureTick);
        _onCooldown = false;

    }

    public void ReduceCapture(Building target)
    {
        if (target.Damage[Owner] < Math.Abs(Marketing))
        {
            target.Damage[Owner] = 0;
            Debug.Log(target.Damage[Owner]);
            return;
        }
        target.Damage[Owner] += Marketing;
        Debug.Log(target.Damage[Owner]);
    }

    public void Capture(Building target)
    {
        if (Owner == target.Owner)
        {
            return;
        }

        target.Damage[Owner] += Marketing;
        Debug.Log(target.Damage[Owner]);

        if (target.Damage[Owner] >= target.BuildingInformation.BaseCost)
        {
            target.ChangeOwner(Owner);
        }
    }

    public void Build(int player, GridCell cell, BuildingInformation buildingInformation, bool instant = false)
    {
        Owner = player;
        Cell = cell;
        if (_generateIncome == null)
        {
            _generateIncome = gameObject.AddComponent<GenerateIncome>();
        }
        _generateIncome.Init(this);
        BuildingInformation = buildingInformation;
        _captureTick = 1 / MarketingSpeed;
        BaseCost = BuildingInformation.BaseCost;
        Income = BuildingInformation.Income;
        Marketing = BuildingInformation.InfluenceValue;
        Radius = BuildingInformation.InfluenceRadius;

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
        else
        {
            HandleBuildComplete();
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
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(false);
        }
        Build(Owner, Cell, BuildingInformation.Evolution);
    }

    public void Downgrade()
    {
        Build(Owner, Cell, BuildingInformation.Previous);
    }

    public void Update()
    {
        //checks if target is allied or enemy
        bool targetAllied = Marketing < 0;
        if (Owner == 0 || Marketing == 0)
        {
            
        }

        else if (_searchingTarget)
        {
            Targets = GetTargets(targetAllied);
            StartCoroutine(SearchTargetsTick());
        }

        if (!_onCooldown && !Deactivated && Targets != null)
        {
            if (targetAllied)
            {
                foreach (var target in Targets)
                {
                    ReduceCapture(target);
                }
            }

            else
            {
                foreach (var target in Targets) 
                {
                    Capture(target);
                }
            }
             
            StartCoroutine(CaptureTick());
        }

    }

}
