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

    private float marketing;

    private bool _searchingTarget = true;

    private float _searchTargetTick = 0.1f;

    private float _captureTick = 1f;
 
    private bool _onCooldown = false;
    private GenerateIncome _generateIncome;
    private bool _rangeActive = false;
    private bool _initialBuild = true;
    [SerializeField] GameObject _particleSystemPrefab;
    private ParticleSystem _particleSystem;
    private bool wasDamaged = false;

    public static event Action<Building> OnBuildingConstructed;
    public static event Action<Building, int, int> OnBuildingCaptured;
    public static event Action<Building, int> OnBuildingUpgraded;

    public bool IsAllied(Building target)
    {
        return Owner == target.Owner;
    }

    public bool IsDamaged()
    {
        for (int i = 1; i <= LevelManager.Instance.NumPlayers; i++)
        {
            if (Damage[i] > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool InRange(Building target)
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
                if (target.Cell.Position == new Vector2Int(Math.Clamp(Cell.Position.x + x,0,LevelManager.Instance.MapWidth-1), 
                                                           Math.Clamp(Cell.Position.y + y,0, LevelManager.Instance.MapHeight-1)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int DistanceTo(Building Target)
    {
        return((Target.Cell.Position.x - Cell.Position.x) * (Target.Cell.Position.x - Cell.Position.x)+ 
               (Target.Cell.Position.y - Cell.Position.y) * (Target.Cell.Position.y - Cell.Position.y));
    }

    public int DistanceTo(Vector2Int Target)
    {
        return((Target[0] - Cell.Position.x) * (Target[0] - Cell.Position.x)+ 
               (Target[1] - Cell.Position.y) * (Target[1] - Cell.Position.y));
    }

    public List<Vector2Int> GetInflucenceCellCoordinates()
    {
        List<Vector2Int> cell_list = new List<Vector2Int>();
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

                var xPos = Cell.Position.x + x;
                var yPos = Cell.Position.y + y;

                if (xPos >= LevelManager.Instance.MapWidth || yPos >= LevelManager.Instance.MapHeight
                    || xPos < 0 || yPos < 0)
                {
                    continue;
                }

                if (cells[xPos,yPos].CellType == GridCell.CellTypes.Buildable && cells[xPos, yPos].ConstructedBuilding == null)
                    cell_list.Add(new Vector2Int(xPos,yPos));
            }
        }
        return cell_list;
    }

    public List<Building> GetTargets(bool isAllied)
    {
        
        var cells = LevelManager.Instance.GridController.Cells;
        var targetList = new List<Building>();
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

                if (xPos >= LevelManager.Instance.MapWidth || yPos >= LevelManager.Instance.MapHeight
                    || xPos < 0 || yPos < 0)
                {
                    continue;
                }

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

        int radius = BuildingInformation.InfluenceRadius;
        var cells = LevelManager.Instance.GridController.Cells;
        // range logic
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
        int oldOwner = Owner;
        LevelManager.Instance.NumBuildings[Owner]--;

        for (int i = 1; i <= LevelManager.Instance.NumPlayers; i++)
        {
            Damage[i] = 0;
        }


        _generateIncome.ToggleIncome(false);
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(false);
        }

        Build(player, Cell, BuildingInformation, false);
        LevelManager.Instance.NumBuildings[Owner]++;

        OnBuildingCaptured?.Invoke(this, oldOwner, Owner);
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
        for (int i = 1; i <= LevelManager.Instance.NumPlayers; i++)
        {
            if (target.Damage[i] < Math.Abs(marketing))
            {
                target.Damage[i] = 0;
                return;
            }
            target.Damage[i] -= marketing;
        }
    }

    public void Capture(Building target)
    {
        if (Owner == target.Owner)
        {
            return;
        }

        target.Damage[Owner] += marketing;

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
        if (_particleSystem == null)
        {
            _particleSystem = Instantiate(_particleSystemPrefab).GetComponent<ParticleSystem>();
        }
        _particleSystem.GetComponent<Transform>().position = LevelManager.Instance.GridController.GetWorldPos(Cell.Position);
        var main = _particleSystem.main;
        if (Owner == 0)
        {
            main.startColor = Color.white;
        }
        else if (Owner == 1)
        {
            main.startColor = Color.blue;
        }
        else if (Owner == 2)
        {
            main.startColor = Color.red;
        }
        else if (Owner == 3)
        {
            main.startColor = Color.yellow;
        }
        _particleSystem.Stop();
        BuildingInformation = buildingInformation;
        _captureTick = 1f / MarketingSpeed;
        marketing = BuildingInformation.InfluenceValue;
        LevelManager.Instance.GridController.SetBuilding(cell, this, player);

        if (_initialBuild)
        {
            _initialBuild = false;
            OnBuildingConstructed?.Invoke(this);
        }

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
        _generateIncome.ToggleIncome(false);
    }

    public void Activate()
    {
        Deactivated = false;
        LevelManager.Instance.GridController.SetTileColor(Cell.Position, new Color(1f, 1f, 1f));
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(true);
        }
        _generateIncome.ToggleIncome(true);
    }

    public void Upgrade()
    {
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(false);
        }
        _generateIncome.ToggleIncome(false);
        Build(Owner, Cell, BuildingInformation.Evolution);
        OnBuildingUpgraded?.Invoke(this, Owner);
    }

    public void Downgrade()
    {
        if (BuildingInformation.PermitsBuildingWithinRange)
        {
            ToggleBuilding(false);
        }
        _generateIncome.ToggleIncome(false);
        Build(Owner, Cell, BuildingInformation.Previous);
    }

    public void Update()
    {
        //checks if target is allied or enemy
        bool targetAllied = marketing < 0;
        if (Owner == 0 || marketing == 0)
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

        if (_particleSystem != null)
        {
            bool isDamaged = IsDamaged();
            if (isDamaged && !wasDamaged)
            {
                _particleSystem.Play();
            }
            else if (wasDamaged && !isDamaged)
            {
                _particleSystem.Stop();
            }
            wasDamaged = isDamaged;
        }
    }

}
