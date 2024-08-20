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
    [SerializeField] private AudioClip _victorySound;
    [SerializeField] private AudioClip _defeatSound;
    public GameObject aiManagerPrefab;

    public static LevelManager Instance { get; private set; }
    public float BriberyFactor = 5.0f;
    public GridController GridController => _gridController;
    public int NumPlayers => _numPlayers;
    public Dictionary<int, float> Currencies { get; set; } = new Dictionary<int, float>();
    public Dictionary<int, float> Incomes { get; set; } = new Dictionary<int, float>();
    public Dictionary<int, int> NumBuildings { get; set; } = new Dictionary<int, int>();
    public float BankruptcyTimeLimit { get; set; } = 30f;
    public int MapWidth => _mapWidth;
    public int MapHeight => _mapHeight;
    public GridCell Selected { get; set; }
    public float TimeBeforeBankruptcy => _timeBeforeBankruptcy;
    public float PlayerIncome => Incomes[1];

    private ILevelInitializer _levelInitializer;
    private float _timeBeforeBankruptcy;
    private bool _negative = false;


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

    public Building ConstructBuilding(int player, GridCell cell, BuildingInformation buildingInformation, bool free=false, bool instant = false)
    {
        if (!free)
        {
            Currencies[player] -= CalculateCost(player, cell, buildingInformation);
        }
        Building building = new GameObject("Building").AddComponent<Building>();
        building.Build(player, cell, buildingInformation, instant);
        NumBuildings[player]++;
        return building;
    }

    public void Victory()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ShowVictoryScreen();
        AudioManager.Instance.PlayEffect(_victorySound);
    }

    public void Defeat()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ShowDefeatScreen();
        AudioManager.Instance.PlayEffect(_defeatSound);
    }

    public void UpgradeBuilding(GridCell cell)
    {
        Building building = cell.ConstructedBuilding;
        Currencies[building.Owner] -= CalculateCost(building.Owner, building.Cell, building.BuildingInformation.Evolution);
        building.Upgrade();
    }

    public void DowngradeBuilding(GridCell cell)
    {
        Building building = cell.ConstructedBuilding;
        Currencies[building.Owner] += CalculateCost(building.Owner, building.Cell, building.BuildingInformation)/2;
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
            Incomes[i] = 0;
        }

        GridController.Cells = new GridCell[_mapWidth, _mapHeight];
        GridController.InitializeCells();

        _levelInitializer = GetComponent<ILevelInitializer>();
        _levelInitializer.InitializeLevel();

        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        float previousPlayerCurrency = Currencies[1];

        for (int i = 1; i <= _numPlayers; i++)
        {
            Currencies[i] += Incomes[i] * Time.deltaTime;
        }

        if (Currencies[1] < 0 && previousPlayerCurrency > 0)
        {
            _negative = true;
            _timeBeforeBankruptcy = BankruptcyTimeLimit;
            UIManager.Instance.ShowBankruptcyTimer();
        }
        else if (previousPlayerCurrency < 0 && Currencies[1] > 0)
        {
            _negative = false;
            UIManager.Instance.HideBankruptcyTimer();
        }

        if (_negative)
        {
            _timeBeforeBankruptcy -= Time.deltaTime;
            if (_timeBeforeBankruptcy <= 0)
            {
                Defeat();
            }
        }
    }
}
