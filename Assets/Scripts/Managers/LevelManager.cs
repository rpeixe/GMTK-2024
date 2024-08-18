using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GridController _gridController;
    [SerializeField] private int _numPlayers = 1;
    [SerializeField] private float _initialMoney = 10f;

    public static LevelManager Instance { get; private set; }
    public GridController GridController => _gridController;
    public int NumPlayers => _numPlayers;
    public Dictionary<int, float> Currencies { get; set; } = new Dictionary<int, float>();
    public Dictionary<int, int> NumBuildings { get; set; } = new Dictionary<int, int>();
    public GridCell Selected { get; set; }

    public void AddCurrency(int player, float currency)
    {
        Currencies[player] += currency;
    }

    public void RemoveCurrency(int player, float currency)
    {
        Currencies[player] -= currency;
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

    public void ConstructBuilding(int player, GridCell cell, BuildingInformation buildingInformation)
    {
        RemoveCurrency(player, CalculateCost(player, cell, buildingInformation));
        Building building = new GameObject("Building").AddComponent<Building>();
        building.Build(player, cell, buildingInformation);
        NumBuildings[player]++;
    }

    public void Victory()
    {

    }

    public void Defeat()
    {
    
    }

    public void UpgradeBuilding(GridCell cell)
    {
        Building building = cell.ConstructedBuilding;
        RemoveCurrency(building.Owner, CalculateCost(building.Owner, building.Cell, building.BuildingInformation));
        building.Upgrade();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 1; i <= _numPlayers; i++)
        {
            Currencies[i] = _initialMoney;
            NumBuildings[i] = 0;
        }
    }
}
