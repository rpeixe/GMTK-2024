using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int[][] coords;
    public bool[] achieved;

    public void InitiateGoals(int[][] goal_coords)
    {
	int n = goal_coords.Length;
	coords = new int[n][];
	achieved = new bool[n];
	for (int i = 0; i < n; ++i)
	{
	    coords[i] = new int[2];
	    coords[i][0] = goal_coords[i][0];
	    coords[i][1] = goal_coords[i][1];
	    achieved[i] = false;
	}
    }

    public void SetGoal(int goal_id, bool has_achieved = true) {
        achieved[goal_id] = has_achieved;
    }

    public int[] NextGoal() {
        int n = achieved.Length;
	int[] next_goal = new int[2] {-1,-1};
        for (int i = 0; i < n; ++i)
	{
	    if (!achieved[i]) {
	       next_goal[0] = coords[i][0];
	       next_goal[1] = coords[i][1];
	       return(next_goal);
	    }
	    
	}
	return(next_goal);
    }

}
