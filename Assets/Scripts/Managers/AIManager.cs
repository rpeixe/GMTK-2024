using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private BuildingInformation _officeBuilding;
    [SerializeField] private BuildingInformation _billboardBuilding;
    [SerializeField] private BuildingInformation _ornamentalBuilding;
    [SerializeField] private BuildingInformation _entertainmentBuilding;
    public int MyID;
    public Dictionary<int, Building> HQs;
    public Dictionary<int, List<Building>> buildings_list { get; set; } = new Dictionary<int, List<Building>>();
    public List<Building> expansion_buildings { get; set; } = new List<Building>();
    public List<int> buildings_dist { get; set; } = new List<int>();
    public List<Building> GoalBuildings;
    public Building CurrentGoal;
    public bool CurrentGoalInRange = false;
    public bool WarStarted = false;
    public bool IsBerzerk = false;
    public bool InPanic = false;

    AIBaseState currentState;
    public AIPeaceState PeaceState = new AIPeaceState();
    public AIWarState WarState = new AIWarState();
    public AIBerzerkState BerzerkState = new AIBerzerkState();
    public AIPanicState PanicState = new AIPanicState();

    void Start()
    {
        currentState = PeaceState;
        currentState.EnterState(this);
    }

    private void OnEnable()
    {
        Building.OnBuildingConstructed += AddBuilding;
        Building.OnBuildingCaptured += ConquerBuilding;
    }

    private void OnDisable()
    {
        Building.OnBuildingConstructed -= AddBuilding;
        Building.OnBuildingCaptured -= ConquerBuilding;
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    // Called to change state. Always called by the current AIBaseState
    public void SwitchState(AIBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    // Must be initialized in the begining of the level.
    public void setPlayers(int ID)
    {
        MyID = ID;
        for (int i = 0; i <= LevelManager.Instance.NumPlayers; ++i)
            buildings_list.Add(i, new List<Building>());
    }

    // Must be initialized in the begining of the level.
    public void SetHQs(Dictionary<int, Building> level_HQs)
    {
        HQs = level_HQs;
    }

    // Must be initialized in the begining of the level.
    public void SetGoals(List<Building> goal_buildings)
    {
        GoalBuildings = goal_buildings;
        NextGoal();
    }

    public void StartPlaying()
    {
        InvokeRepeating(nameof(ProcessAI), 2.0f, 2.0f);
    }

    // Called to check if goal building changed after a building is added to a player.
    public void NextGoal()
    {
        int n_goals = GoalBuildings.Count;
        for (int i = 0; i < n_goals; ++i)
        {
            if (GoalBuildings[i].Owner != MyID)
            {
                CurrentGoal = GoalBuildings[i];
                GoalChanged();
                break;
            }
        }
        int n = buildings_list[MyID].Count;
        for (int i = 0; i < n; ++i)
        {
            if (buildings_list[MyID][i].InRange(CurrentGoal))
            {
                CurrentGoalInRange = true;
                return;
            }
        }
        CurrentGoalInRange = false;
    }

    void GoalChanged() {
        for (int i = 0; i < buildings_dist.Count; ++i)
        {
            Vector2Int coords = expansion_buildings[i].Cell.Position;
            Vector2Int goal_coords = CurrentGoal.Cell.Position;
            buildings_dist[i] = (coords[0] - goal_coords[0]) * (coords[0] - goal_coords[0]) +
                                (coords[1] - goal_coords[1]) * (coords[1] - goal_coords[1]);
        }
        for (int i = buildings_dist.Count - 1; i > 0; --i)
        {
            for (int j = i - 1; j >= 0; --j)
            {
                if (buildings_dist[j] > buildings_dist[i])
                {
                    Building tmp_building = expansion_buildings[j];
                    int tmp_distance = buildings_dist[j];
                    expansion_buildings[j] = expansion_buildings[i];
                    buildings_dist[j] = buildings_dist[i];
                    expansion_buildings[i] = tmp_building;
                    buildings_dist[i] = tmp_distance;
                }
            }
        }
    }

    // Called when AI adds a new building, to see if target is in range.
    public void CheckIsBerzerk(Building new_building)
    {
        for (int i = 0; i < GoalBuildings.Count; ++i)
        {
            if ((new_building.InRange(GoalBuildings[i]) && (GoalBuildings[1].Owner != MyID)))
            {
                IsBerzerk = true;
                return;
            }
        }
    }

    public bool HasEnemyBuildingInRange(Building my_building)
    {
        for (int player = 1; player <= LevelManager.Instance.NumPlayers; ++player)
        {
            if (player == MyID)
                continue;
            int n = buildings_list[player].Count;
            for (int i = 0; i < n; ++i)
            {
                if ((buildings_list[player][i].InRange(my_building)) || (my_building.InRange(buildings_list[player][i])))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Called when AI loses a building. It may happen to lose a goal building or the last building in the range of a goal buinding.
    public void CheckIsBerzerk()
    {
        int n = buildings_list[MyID].Count;
        IsBerzerk = false;
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < GoalBuildings.Count; ++j)
            {
                if (buildings_list[MyID][i].InRange(GoalBuildings[j]))
                {
                    IsBerzerk = true;
                    return;
                }
            }
        }
    }

    // Function to check if war started. This will change AI behavior from peace to war. Peace state never comes back.
    public void CheckCombatRange(Building new_building)
    {
        if (new_building.Owner == MyID)
        {
            for (int player = 1; player <= LevelManager.Instance.NumPlayers; ++player)
            {
                if (player == MyID)
                    continue;
                int n = buildings_list[player].Count;
                for (int i = 1; i < n; ++i)
                {
                    if ((buildings_list[player][i].InRange(new_building)) || (new_building.InRange(buildings_list[player][i])))
                    {
                        WarStarted = true;
                        return;
                    }
                }
            }
        }
        else
        {
            int n = buildings_list[MyID].Count;
            for (int i = 0; i < n; ++i)
            {
                if (buildings_list[MyID][i].InRange(new_building))
                {
                    WarStarted = true;
                    return;
                }
            }
        }
    }

    void CheckPanic()
    {
        for (int player = 1; player <= LevelManager.Instance.NumPlayers; ++player)
        {
            if (player == MyID)
                continue;
            int n = buildings_list[player].Count;
            for (int i = 0; i < n; ++i)
            {
                if (buildings_list[player][i].InRange(HQs[MyID]))
                {
                    InPanic = true;
                    return;
                }
            }
        }
        InPanic = false;
    }

    void CheckPanic(Building new_building)
    {
        if (new_building.InRange(HQs[MyID]))
            InPanic = true;
    }

    // Called when a build is bought or conquered.
    public void AddBuilding(Building new_building)
    {
        int new_building_id = new_building.Owner;
        buildings_list[new_building_id].Add(new_building);
        if (new_building_id == MyID)
        {
            if (new_building.BuildingInformation.Type != BuildingInformation.BuildingType.office) 
            {
                expansion_buildings.Add(new_building);
                Vector2Int coords = new_building.Cell.Position;
                Vector2Int goal_coords = CurrentGoal.Cell.Position;
                int new_dist = (coords[0] - goal_coords[0]) * (coords[0] - goal_coords[0]) +
                               (coords[1] - goal_coords[1]) * (coords[1] - goal_coords[1]);
                buildings_dist.Add(new_dist);
                for (int i = buildings_dist.Count - 1; i > 0; --i)
                {
                    if (buildings_dist[i] < buildings_dist[i - 1])
                    {
                        Building tmp_building = expansion_buildings[i - 1];
                        int tmp_distance = buildings_dist[i - 1];
                        expansion_buildings[i - 1] = expansion_buildings[i];
                        buildings_dist[i - 1] = buildings_dist[i];
                        expansion_buildings[i] = tmp_building;
                        buildings_dist[i] = tmp_distance;
                    }
                }
            }
            if (!IsBerzerk)
                CheckIsBerzerk(new_building);
        }
        else
        {
            if (!InPanic)
                CheckPanic(new_building);
        }
        CheckCombatRange(new_building);
        NextGoal();
    }

    // Called when a building is conquered.
    public void RemoveBuilding(Building remove_building, int id)
    {
        if (id != MyID)
        {
            buildings_list[id].Remove(remove_building);
            if (InPanic)
                CheckPanic();
        }
        else
        {
            NextGoal();
            int n = buildings_list[id].Count;
            for (int i = 0; i < n; ++i)
            {
                if (buildings_list[id][i] == remove_building)
                {
                    buildings_list[id].RemoveAt(i);
                    break;
                }
            }
            n = expansion_buildings.Count;
            for (int i = 0; i < n; ++i)
            {
                if (expansion_buildings[i] == remove_building)
                {
                    expansion_buildings.RemoveAt(i);
                    buildings_dist.RemoveAt(i);
                    break;
                }
            }
            if (IsBerzerk)
                CheckIsBerzerk();
        }
    }

    // Called when a building is conquered.
    public void ConquerBuilding(Building new_building, int old_owner, int new_owner)
    {
        RemoveBuilding(new_building, old_owner);
        AddBuilding(new_building);
    }

    // Logic to determine the building zone when deciding to buy a new defensive building.
    public Building SelectDefensiveBuilding()
    {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
        int n = expansion_buildings.Count;
        switch (n)
        {
            case 0:
                return null;
            case 1:
                return expansion_buildings[0];
            case 2:
                if (prob < 90)
                    return expansion_buildings[1];
                return expansion_buildings[0];
            case 3:
                if (prob < 80)
                    return expansion_buildings[2];
                else if (prob < 95)
                    return expansion_buildings[1];
                return expansion_buildings[0];
            case 4:
                if (prob < 75)
                    return expansion_buildings[3];
                else if (prob < 90)
                    return expansion_buildings[2];
                else if (prob < 97)
                    return expansion_buildings[1];
                return expansion_buildings[0];
            case 5:
                if (prob < 70)
                    return expansion_buildings[4];
                else if (prob < 85)
                    return expansion_buildings[3];
                else if (prob < 92)
                    return expansion_buildings[2];
                else if (prob < 98)
                    return expansion_buildings[1];
                return expansion_buildings[0];
            default:
                if (prob < 60)
                    return expansion_buildings[n - 1];
                else if (prob < 80)
                    return expansion_buildings[n - 2];
                else if (prob < 90)
                    return expansion_buildings[n - 3];
                else if (prob < 96)
                    return expansion_buildings[n - 4];
                else if (prob < 99)
                    return expansion_buildings[n - 5];
                return expansion_buildings[n - 6];
        }
    }

    // Logic to determine the building zone when deciding to buy a new attack building.    
    public Building SelectOffensiveBuilding()
    {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
        Debug.Log($"expansion_buildings.Count: {expansion_buildings.Count}");
        switch (expansion_buildings.Count)
        {
            case 0:
                return null;
            case 1:
                return expansion_buildings[0];
            case 2:
                if (prob < 90)
                    return expansion_buildings[0];
                return expansion_buildings[1];
            case 3:
                if (prob < 80)
                    return expansion_buildings[0];
                else if (prob < 95)
                    return expansion_buildings[1];
                return expansion_buildings[2];
            case 4:
                if (prob < 75)
                    return expansion_buildings[0];
                else if (prob < 90)
                    return expansion_buildings[1];
                else if (prob < 97)
                    return expansion_buildings[2];
                return expansion_buildings[3];
            case 5:
                if (prob < 70)
                    return expansion_buildings[0];
                else if (prob < 85)
                    return expansion_buildings[1];
                else if (prob < 92)
                    return expansion_buildings[2];
                else if (prob < 98)
                    return expansion_buildings[3];
                return expansion_buildings[4];
            default:
                if (prob < 60)
                    return expansion_buildings[0];
                else if (prob < 80)
                    return expansion_buildings[1];
                else if (prob < 90)
                    return expansion_buildings[2];
                else if (prob < 96)
                    return expansion_buildings[3];
                else if (prob < 99)
                    return expansion_buildings[4];
                return expansion_buildings[5];
        }
    }

    // Logic to determine the building to bribe.    
    public Building SelectBuildingToBribe()
    {
        int ai_buildings = expansion_buildings.Count;
        for (int player = 1; player <= LevelManager.Instance.NumPlayers; ++player)
        {
            if (player == MyID)
                continue;
            int player_buildings = buildings_list[player].Count;
            for (int i = 0; i < player_buildings; ++i)
            {
                for (int j = 0; j < ai_buildings; ++j)
                {
                    if ((expansion_buildings[j].InRange(buildings_list[player][i])) &&
                        (LevelManager.Instance.BriberyFactor * LevelManager.Instance.CalculateCost(MyID, buildings_list[player][i]) <=
                            LevelManager.Instance.Currencies[MyID]))
                        return buildings_list[player][i];
                }
            }
        }
        return null;
    }

    public Vector2Int NewOfficeCoordinates(Building reference_building)
    {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1, -1);
        int max_dist = 0;
        List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
        if (cells.Count == 0)
            return coords;
        for (int i = 0; i < System.Math.Min(cells.Count, 4); ++i)
        {
            int sel = rnd.Next(cells.Count);
            int dist = HQs[1].DistanceTo(cells[sel]);
            // Precisa fazer o build information de office.
            if ((max_dist < dist) &&
                (LevelManager.Instance.CalculateCost(MyID, cells[sel], _officeBuilding) <= LevelManager.Instance.Currencies[MyID]))
            {
                max_dist = dist;
                coords = cells[sel];
            }
        }
        return (coords);
    }

    public Vector2Int NewDefensiveCoordinates(Building reference_building, string building_type)
    {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1, -1);
        List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
        if (cells.Count == 0)
            return coords;
        for (int i = 0; i < cells.Count; ++i)
        {
            int sel = rnd.Next(cells.Count);
            float build_cost;
            if (building_type == "Billboard")
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _billboardBuilding);
            else if (building_type == "Ornamental")
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _ornamentalBuilding);
            else
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _entertainmentBuilding);
            if (build_cost <= LevelManager.Instance.Currencies[MyID])
            {
                coords = cells[sel];
                break;
            }
        }
        return (coords);
    }

    public Vector2Int NewExpansionCoordinates(Building reference_building, string building_type)
    {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1, -1);
        int min_dist = 999;
        List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
        if (cells.Count == 0)
            return coords;
        for (int i = 0; i < 8; ++i)
        {
            int sel = rnd.Next(cells.Count);
            int dist = CurrentGoal.DistanceTo(cells[sel]);
            float build_cost;
            if (building_type == "Billboard")
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _billboardBuilding);
            else if (building_type == "Ornamental")
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _ornamentalBuilding);
            else
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _entertainmentBuilding);
            if ((min_dist > dist) && (build_cost <= LevelManager.Instance.Currencies[MyID]))
            {
                min_dist = dist;
                coords = cells[sel];
            }
        }
        return (coords);
    }

    public Vector2Int NewOffensiveCoordinates(Building reference_building, string building_type)
    {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1, -1);
        int min_dist = 999;
        List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
        if (cells.Count == 0)
            return coords;
        for (int i = 0; i < System.Math.Min(cells.Count, 4); ++i)
        {
            int sel = rnd.Next(cells.Count);
            int dist = reference_building.DistanceTo(cells[sel]);
            float build_cost;
            if (building_type == "Billboard")
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _billboardBuilding);
            else if (building_type == "Ornamental")
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _ornamentalBuilding);
            else
                build_cost = LevelManager.Instance.CalculateCost(MyID, cells[sel], _entertainmentBuilding);
            if ((min_dist > dist) && (build_cost <= LevelManager.Instance.Currencies[MyID]))
            {
                min_dist = dist;
                coords = cells[sel];
            }
        }
        return (coords);
    }

    public void CreateBuilding(Vector2Int location, string building_type)
    {
        if (building_type == "Office")
            LevelManager.Instance.ConstructBuilding(MyID, LevelManager.Instance.GridController.Cells[location[0], location[1]], _officeBuilding);
        else if (building_type == "Billboard")
            LevelManager.Instance.ConstructBuilding(MyID, LevelManager.Instance.GridController.Cells[location[0], location[1]], _billboardBuilding);
        else if (building_type == "Ornamental")
            LevelManager.Instance.ConstructBuilding(MyID, LevelManager.Instance.GridController.Cells[location[0], location[1]], _ornamentalBuilding);
        else
            LevelManager.Instance.ConstructBuilding(MyID, LevelManager.Instance.GridController.Cells[location[0], location[1]], _entertainmentBuilding);
    }

    public void ProcessAI()
    {
        CheckDowngrades();
        currentState.SelectAction(this);
        Debug.Log($"State: {currentState.GetType()}");
        Debug.Log($"Currency: {LevelManager.Instance.Currencies[MyID]}");
        Debug.Log($"Income: {LevelManager.Instance.Incomes[MyID]}");
    }

    public void CheckDowngrades()
    {
        int n = expansion_buildings.Count;
        for (int i = 0; i < n; ++i)
        {
            if (expansion_buildings[i].BuildingInformation.Previous == null)
            {
                continue;
            }
            System.Random rnd = new System.Random();
            int prob = rnd.Next(20);
            List<Building> targets = expansion_buildings[i].GetTargets(false);
            if ((targets.Count == 0) && (prob == 0) &&
               LevelManager.Instance.CalculateCost(expansion_buildings[i].Owner, expansion_buildings[i].Cell,
               expansion_buildings[i].BuildingInformation) / 2 <= LevelManager.Instance.Currencies[MyID])
            {
                LevelManager.Instance.DowngradeBuilding(expansion_buildings[i].Cell);
            }
        }
    }

}


