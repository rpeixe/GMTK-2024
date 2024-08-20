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
    public Dictionary<int,Building> HQs;
    public Dictionary<int,List<Building>> buildings_list { get; set; } = new Dictionary<int,List<Building>>();
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
    public AIManager manager = new AIManager();

    void Start()
    {
        currentState = PeaceState;
        currentState.EnterState(this);
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
    	for (int i = 0; i < LevelManager.Instance.NumPlayers; ++i) 
	    buildings_list.Add(i,new List<Building>());
    }

    // Must be initialized in the begining of the level.
    public void SetHQs(Dictionary<int,Building> level_HQs)
    {
        HQs = level_HQs;
    }

    // Must be initialized in the begining of the level.
    public void SetGoals(List<Building> goal_buildings) 
    {
	GoalBuildings = goal_buildings;
    }

    // Called to check if goal building changed after a building is added to a player.
    public void NextGoal() {
    	int n_goals = GoalBuildings.Count;
        for (int i = 0; i < n_goals; ++i)
	{
	    if (GoalBuildings[i].Owner == MyID) {
	    	CurrentGoal = GoalBuildings[i];
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

    // Called when AI adds a new building, to see if target is in range.
    public void CheckIsBerzerk(Building new_building)
    {
        for(int i = 0; i < GoalBuildings.Count; ++i) 
	{
    	    if ((new_building.InRange(GoalBuildings[i]) && (GoalBuildings[1].Owner != MyID)))
    	    {
    	        IsBerzerk = true;
    	        return;
    	    }
    	}
    }
    
    // Called when AI loses a building. It may happen to lose a goal building or the last building in the range of a goal buinding.
    public void CheckIsBerzerk() {
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
    	    for(int player = 0; player < LevelManager.Instance.NumPlayers; ++player)
    	    {
    	        if (player == MyID)
    	            continue;
    	        int n = buildings_list[player].Count;
    	        for (int i = 0; i < n; ++i) 
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
    	        if (new_building.InRange(new_building))
    	        {
    	            WarStarted = true;
    	            return;
    	        }
    	    }
    	}
    }

    void CheckPanic()
    {
	for(int player = 0; player < LevelManager.Instance.NumPlayers; ++player)
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
            Vector2Int coords = new_building.Cell.Position;
            Vector2Int goal_coords = CurrentGoal.Cell.Position;
 	    int new_dist = (coords[0]-goal_coords[0])*(coords[0]-goal_coords[0])+
	                   (coords[1]-goal_coords[1])*(coords[1]-goal_coords[1]);
            buildings_list[MyID].Add(new_building);
	    buildings_dist.Add(new_dist);
	    for (int i = buildings_list.Count-1; i > 0 ; --i)
	    {
	        if (buildings_dist[i] < buildings_dist[i-1]) 
	    	{
	           Building tmp_building = buildings_list[MyID][i-1];
		   int tmp_distance = buildings_dist[i-1];
	           buildings_list[MyID][i-1] = buildings_list[MyID][i];
		   buildings_dist[i-1] = buildings_dist[i];
		   buildings_list[MyID][i] = tmp_building;
		   buildings_dist[i] = tmp_distance;
		}
	    }
	    if (!IsBerzerk)
	        CheckIsBerzerk(new_building);
	}
	else {
	    if (!InPanic)
	        CheckPanic(new_building);
	}
        CheckCombatRange(new_building);
	NextGoal();
    }

    // Called when a building is conquered.
    public void RemoveBuilding(Building remove_building, int id) {
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
                    buildings_dist.RemoveAt(i);
                }
            }
            if (IsBerzerk)
	        CheckIsBerzerk();
        }
    }

    // Called when a building is conquered.
    public void ConquerBuilding(Building new_building, int old_owner) {
        RemoveBuilding(new_building, old_owner);
        AddBuilding(new_building);
    }

    // Logic to determine the building zone when deciding to buy a new defensive building.
    public Building SelectDefensiveBuilding() {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
        int n = buildings_list[MyID].Count;
    	switch (n)
    	{
    	case 0:
    	    return null;
    	case 1:
    	    return buildings_list[MyID][0];
    	case 2:
    	    if (prob < 90)
    	        return buildings_list[MyID][1];
    	    return buildings_list[MyID][0];
    	case 3:
    	    if (prob < 80)
    	        return buildings_list[MyID][2];
    	    else if (prob < 95)
    	        return buildings_list[MyID][1];
    	    return buildings_list[MyID][0];
    	case 4:
    	    if (prob < 75)
    	        return buildings_list[MyID][3];
    	    else if (prob < 90)
    	        return buildings_list[MyID][2];
    	    else if (prob < 97)
    	        return buildings_list[MyID][1];
    	    return buildings_list[MyID][0];
     	case 5:
    	    if (prob < 70)
    	        return buildings_list[MyID][4];
    	    else if (prob < 85)
    	        return buildings_list[MyID][3];
    	    else if (prob < 92)
    	        return buildings_list[MyID][2];
    	    else if (prob < 98)
    	        return buildings_list[MyID][1];
    	    return buildings_list[MyID][0];
    	default:
    	    if (prob < 60)
    	        return buildings_list[MyID][n-1];
    	    else if (prob < 80)
    	        return buildings_list[MyID][n-2];
    	    else if (prob < 90)
    	        return buildings_list[MyID][n-3];
    	    else if (prob < 96)
    	        return buildings_list[MyID][n-4];
    	    else if (prob < 99)
    	        return buildings_list[MyID][n-5];
    	    return buildings_list[MyID][n-6];
    	}
    }
    
    // Logic to determine the building zone when deciding to buy a new attack building.    
    public Building SelectOffensiveBuilding() {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
    	switch (buildings_list[MyID].Count)
    	{
    	case 0:
    	    return null;
    	case 1:
    	    return buildings_list[MyID][0];
    	case 2:
    	    if (prob < 90)
    	        return buildings_list[MyID][0];
    	    return buildings_list[MyID][1];
    	case 3:
    	    if (prob < 80)
    	        return buildings_list[MyID][0];
    	    else if (prob < 95)
    	        return buildings_list[MyID][1];
    	    return buildings_list[MyID][2];
    	case 4:
    	    if (prob < 75)
    	        return buildings_list[MyID][0];
    	    else if (prob < 90)
    	        return buildings_list[MyID][1];
    	    else if (prob < 97)
    	        return buildings_list[MyID][2];
    	    return buildings_list[MyID][3];
     	case 5:
    	    if (prob < 70)
    	        return buildings_list[MyID][0];
    	    else if (prob < 85)
    	        return buildings_list[MyID][1];
    	    else if (prob < 92)
    	        return buildings_list[MyID][2];
    	    else if (prob < 98)
    	        return buildings_list[MyID][3];
    	    return buildings_list[MyID][4];
    	default:
    	    if (prob < 60)
    	        return buildings_list[MyID][0];
    	    else if (prob < 80)
    	        return buildings_list[MyID][1];
    	    else if (prob < 90)
    	        return buildings_list[MyID][2];
    	    else if (prob < 96)
    	        return buildings_list[MyID][3];
    	    else if (prob < 99)
    	        return buildings_list[MyID][4];
    	    return buildings_list[MyID][5];
    	}
    }

    // Logic to determine the building to bribe.    
    public Building SelectBuildingToBribe() {
        int ai_buildings = buildings_list[MyID].Count;
    	for(int player = 0; player < LevelManager.Instance.NumPlayers; ++player)
    	{
    	    if (player == MyID)
    	        continue;
    	    int player_buildings = buildings_list[player].Count;
    	    for (int i = 0; i < player_buildings; ++i) 
    	    {
	        for (int j = 0; j < ai_buildings; ++j) 
    	    	{
    	        if ((buildings_list[MyID][j].InRange(buildings_list[player][i])) && 
    	            (LevelManager.Instance.BriberyFactor * LevelManager.Instance.CalculateCost(MyID,buildings_list[player][i]) <= 
    	            	LevelManager.Instance.Currencies[MyID]))
    	            return buildings_list[player][i];
    	        }
            }
        }
        return null;
    }

    public Vector2Int NewOfficeCoordinates(Building reference_building) {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1,-1);
        int min_dist = 999;
	List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
	for (int i = 0; i < System.Math.Min(cells.Count, 4); ++i) 
	{
	    int sel = rnd.Next(cells.Count);
	    int dist = HQs[(MyID+1) % 2].DistanceTo(cells[sel]);
	    // Precisa fazer o build information de office.
	    if ((min_dist > dist) && 
	        (LevelManager.Instance.CalculateCost(MyID, cells[sel], new BuildingInformation()) <= LevelManager.Instance.Currencies[MyID]))
	    {
	        min_dist = dist;
	        coords = cells[sel];
	    }
        }
        return(coords);
    }

    public Vector2Int NewDefensiveCoordinates(Building reference_building, string building_type) {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1,-1);
	List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
	for (int i = 0; i < cells.Count; ++i) 
	{
	    int sel = rnd.Next(cells.Count);
	    if (LevelManager.Instance.CalculateCost(MyID, cells[sel], new BuildingInformation()) <= LevelManager.Instance.Currencies[MyID])
	    {
	        coords = cells[sel];
	        break;
	    }
        }
        return(coords);
    }

    public Vector2Int NewExpansionCoordinates(Building reference_building, string building_type) {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1,-1);
        int min_dist = 999;
	List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
	for (int i = 0; i < System.Math.Min(cells.Count, 4); ++i) 
	{
	    int sel = rnd.Next(cells.Count);
	    int dist = CurrentGoal.DistanceTo(cells[sel]);
	    // Precisa fazer o build information de office.
	    if ((min_dist > dist) && 
	        (LevelManager.Instance.CalculateCost(MyID, cells[sel], new BuildingInformation()) <= LevelManager.Instance.Currencies[MyID]))
	    {
	        min_dist = dist;
	        coords = cells[sel];
	    }
        }
        return(coords);
    }
    
    public Vector2Int NewOffensiveCoordinates(Building reference_building, string building_type) {
        System.Random rnd = new System.Random();
        Vector2Int coords = new Vector2Int(-1,-1);
        int min_dist = 999;
	List<Vector2Int> cells = reference_building.GetInflucenceCellCoordinates();
	for (int i = 0; i < System.Math.Min(cells.Count, 4); ++i) 
	{
	    int sel = rnd.Next(cells.Count);
	    int dist = reference_building.DistanceTo(cells[sel]);
	    // Precisa fazer o build information de office.
	    if ((min_dist > dist) && 
	        (LevelManager.Instance.CalculateCost(MyID, cells[sel], new BuildingInformation()) <= LevelManager.Instance.Currencies[MyID]))
	    {
	        min_dist = dist;
	        coords = cells[sel];
	    }
        }
        return(coords);
    }

}


