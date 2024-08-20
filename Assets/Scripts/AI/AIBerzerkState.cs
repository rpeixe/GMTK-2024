using UnityEngine;

//Action/State  office	Def	Off	Wait	Bribery
//1. $/sec < 10	50%	0%	50%	0%	0%	(upgrade in range or buy another one close)
//2. others	0%	0%	100%	0%	0%	(upgrade in range or buy another one close)


public class AIBerzerkState : AIBaseState 
{
    public override void EnterState(AIManager ai) 
    {
    }
    public override void UpdateState(AIManager ai) 
    {
        if (ai.InPanic)
            ai.SwitchState(new AIPanicState());
        else if (!ai.IsBerzerk) 
            ai.SwitchState(new AIWarState());
        else if (!ai.WarStarted)
            ai.SwitchState(new AIPeaceState());
    }
    public override void SelectAction(AIManager ai)
    {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
        float current_income = LevelManager.Instance.Incomes[ai.MyID];
        if ((current_income < 10) && (prob < 50)) {
            Building local_building = ai.SelectDefensiveBuilding();
            Vector2Int location = ai.NewOfficeCoordinates(local_building);
            if (location[0] != -1)
                ai.CreateBuilding(location, "Office");
        }
        else {
            Building local_building = ai.SelectOffensiveBuilding();
            int up_prob = rnd.Next(5);
            // Try to upgrade building.
            if ((ai.HasEnemyBuildingInRange(local_building)) && (up_prob == 0) && 
                (LevelManager.Instance.CalculateCost(local_building.Owner, local_building.Cell, local_building.BuildingInformation.Evolution)) <= 
                    LevelManager.Instance.Currencies[ai.MyID])
                LevelManager.Instance.UpgradeBuilding(local_building.Cell);
            string building_type = "Billboard";
            if (ai.buildings_list[ai.MyID].Count > 10) {
	        int type_prob = rnd.Next(6);
	        if (type_prob < 2)
	            building_type = "Ornamental";
	        else if (type_prob < 3)
	            building_type = "Entertainment";
	    }
            Vector2Int location = ai.NewOffensiveCoordinates(local_building, building_type);
            if (location[0] != -1) 
                ai.CreateBuilding(location, building_type);
        }
    }
}
