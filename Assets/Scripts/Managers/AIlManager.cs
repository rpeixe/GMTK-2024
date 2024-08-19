using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public int MyID;
    public Building MyHQ;
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
    public void SetHQ(Building my_HQ)
    {
        MyHQ = my_HQ;
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
    	        if (buildings_list[player][i].InRange(MyHQ))
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
        if (new_building.InRange(MyHQ))
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

    // Logic to determine the expansion incluence zone when deciding to buy a new building.    
    public int SelectExpansionBuilding() {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
    	switch (buildings_list.Count)
    	{
    	case 0:
    	    return -1;
    	case 1:
    	    return 0;
    	case 2:
    	    if (prob < 90)
    	        return 0;
    	    return 1;
    	case 3:
    	    if (prob < 80)
    	        return 0;
    	    else if (prob < 95)
    	        return 1;
    	    return 2;
    	case 4:
    	    if (prob < 75)
    	        return 0;
    	    else if (prob < 90)
    	        return 1;
    	    else if (prob < 97)
    	        return 2;
    	    return 3;
     	case 5:
    	    if (prob < 70)
    	        return 0;
    	    else if (prob < 85)
    	        return 1;
    	    else if (prob < 92)
    	        return 2;
    	    else if (prob < 98)
    	        return 3;
    	    return 4;
    	default:
    	    if (prob < 60)
    	        return 0;
    	    else if (prob < 80)
    	        return 1;
    	    else if (prob < 90)
    	        return 2;
    	    else if (prob < 96)
    	        return 3;
    	    else if (prob < 99)
    	        return 4;
    	    return 5;
    	}
    }
    
    
}


