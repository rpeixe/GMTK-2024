using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GridController _gridController;
    [SerializeField] private int _numPlayers = 1;
    [SerializeField] private float _initialMoney = 10f;
    [SerializeField] private int _mapWidth = 20;
    [SerializeField] private int _mapHeight = 10;
    [SerializeField] private GameObject _victoryScreen;
    [SerializeField] private GameObject _defeatScreen;
    private ILevelInitializer _levelInitializer;

    public static LevelManager Instance { get; private set; }
    public float BriberyFactor = 5.0f;
    public GridController GridController => _gridController;
    public int NumPlayers => _numPlayers;
    public Dictionary<int, float> Currencies { get; set; } = new Dictionary<int, float>();
    public Dictionary<int, float> Incomes { get; set; } = new Dictionary<int, float>();
    public Dictionary<int, Dictionary<string, List<Building>>> Buildings { get; set; } = new Dictionary<int, Dictionary<string, List<Building>>>();
    public Dictionary<int, int> NumBuildings { get; set; } = new Dictionary<int, int>();
    public float BankruptcyTimeLimit { get; set; } = 30f;
    public int MapWidth => _mapWidth;
    public int MapHeight => _mapHeight;
    public GridCell Selected { get; set; }

    public void AddCurrency(int player, float currency)
    {
        float previousCurrency = Currencies[player];
        Currencies[player] += currency;

        if (player == 1 && previousCurrency < 0 && Currencies[player] >= 0)
        {
            CancelInvoke(nameof(Defeat));
        }
    }

    public void RemoveCurrency(int player, float currency)
    {
        float previousCurrency = Currencies[player];
        Currencies[player] -= currency;

        if (player == 1 && Currencies[player] < 0 && previousCurrency >= 0)
        {
            Invoke(nameof(Defeat), BankruptcyTimeLimit);
        }
    }

    public float CalculateCost(int player, Building building)
    {
        return CalculateCost(player, building.Cell, building.BuildingInformation);
    }
    
    public float CalculateCost(int player, Vector2Int pos, BuildingInformation buildingInformation)
    {
        return CalculateCost(player, GridController.Cells[pos.x, pos.y], buildingInformation);
    }

    public float CalculateCost(int player, GridCell cell, BuildingInformation buildingInformation)
    {
        float bc = buildingInformation.BaseCost;
        float cf = cell.RegionClass.CostFactor;
        // float pi = cell.GetInfluencePercentage(player);
        // float aa = cell.GetAntiInfluence(player);
        float af = 0.05f * NumBuildings[player];
        float finalCost = bc * cf * (1 + af);

        return finalCost;
    }

    public void ConstructBuilding(int player, GridCell cell, BuildingInformation buildingInformation, bool free=false, bool instant = false)
    {
        if (!free)
        {
            RemoveCurrency(player, CalculateCost(player, cell, buildingInformation));
        }
        Building building = new GameObject("Building").AddComponent<Building>();
        building.Build(player, cell, buildingInformation, instant);
        NumBuildings[player]++;
    }

    public void Victory()
    {
        UIManager.Instance.Unselect();
        Time.timeScale = 0f;
        _victoryScreen.SetActive(true);
    }

    public void Defeat()
    {
        UIManager.Instance.Unselect();
        Time.timeScale = 0f;
        _defeatScreen.SetActive(true);
    }

    public void UpgradeBuilding(GridCell cell)
    {
        Building building = cell.ConstructedBuilding;
        RemoveCurrency(building.Owner, CalculateCost(building.Owner, building.Cell, building.BuildingInformation.Evolution));
        building.Upgrade();
    }

    public void DowngradeBuilding(GridCell cell)
    {
        Building building = cell.ConstructedBuilding;
        AddCurrency(building.Owner, CalculateCost(building.Owner, building.Cell, building.BuildingInformation)/2);
        building.Downgrade();
    }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i <= _numPlayers; i++)
        {
            Currencies[i] = _initialMoney;
            NumBuildings[i] = 0;
        }

        GridController.Cells = new GridCell[_mapWidth, _mapHeight];
        GridController.InitializeCells();

        _levelInitializer = GetComponent<ILevelInitializer>();
        _levelInitializer.InitializeLevel();

        Time.timeScale = 1.0f;
    }
}
