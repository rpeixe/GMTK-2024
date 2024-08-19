using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public List<Building> HQ { get; set; } = new List<Building>();
    public List<Building> Office { get; set; } = new List<Building>();
    public List<Building> Billboard { get; set; } = new List<Building>();
    public List<Building> Ornamental { get; set; } = new List<Building>();
    public List<Building> Entertainment { get; set; } = new List<Building>();
    public List<Building> Monument { get; set; } = new List<Building>();
    public List<int> HQ_dist { get; set; } = new List<int>();
    public List<int> Office_dist { get; set; } = new List<int>();
    public List<int> Billboard_dist { get; set; } = new List<int>();
    public List<int> Ornamental_dist { get; set; } = new List<int>();
    public List<int> Entertainment_dist { get; set; } = new List<int>();
    public List<int> Monument_dist { get; set; } = new List<int>();
    public Goal goal;
    public int[] current_goal_coords;

    public void SetCurrentGoal() {
        current_goal_coords = goal.NextGoal();
    }

    public void AddBuilding(Building new_building, string building_type) {
        Vector2Int coords = new_building.Cell.Position;
 	int new_dist = (coords[0]-current_goal_coords[0])*(coords[0]-current_goal_coords[0])+
	               (coords[1]-current_goal_coords[1])*(coords[1]-current_goal_coords[1]);
        if (building_type == "HQ")
	    AddBuilding(new_building, new_dist, HQ, HQ_dist);
        else if (building_type == "Office")
	    AddBuilding(new_building, new_dist, Office, Office_dist);
        else if (building_type == "Billboard")
	    AddBuilding(new_building, new_dist, Billboard, Billboard_dist);
        else if (building_type == "Ornamental")
	    AddBuilding(new_building, new_dist, Ornamental, Ornamental_dist);
        else if (building_type == "Entertainment")
	    AddBuilding(new_building, new_dist, Entertainment, Entertainment_dist);
	else
	    AddBuilding(new_building, new_dist, Monument, Monument_dist);
    }

    public void AddBuilding(Building new_building, int new_distance, List<Building> buildings, List<int> distances) {
        buildings.Add(new_building);
	distances.Add(new_distance);
	for (int i = HQ.Count-1; i > 0 ; --i)
	{
	    if (distances[i] < distances[i-1]) {
	        Building tmp_building = buildings[i-1];
		int tmp_distance = distances[i-1];
	        buildings[i-1] = buildings[i];
		distances[i-1] = distances[i];
		buildings[i] = tmp_building;
		distances[i] = tmp_distance;
	    }
	}
        
    }

}
