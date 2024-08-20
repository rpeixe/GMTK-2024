using UnityEngine;

////Action/State        office	Def	Off	Wait	Bribery
//-			20%	75%	0%	0%	5%	(upgrade in range or buy another one close)

public class AIPanicState : AIBaseState 
{
    public override void EnterState(AIManager ai) 
    {
    }
    public override void UpdateState(AIManager ai) 
    {
        if ((!ai.InPanic) && (!ai.IsBerzerk))
            ai.SwitchState(new AIWarState());
        else if (!ai.InPanic)
            ai.SwitchState(new AIBerzerkState());
    }
    public override void SelectAction(AIManager ai)
    {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
        float current_income = LevelManager.Instance.Incomes[ai.MyID];
        if (prob < 20) {
            Building local_building = ai.SelectDefensiveBuilding();
            Vector2Int location = ai.NewOfficeCoordinates(local_building);
            if (location[0] != -1)
                ai.CreateBuilding(location, "Office");
        }
	else if (prob < 95) {
	    Building local_building = ai.SelectDefensiveBuilding();
	    string building_type = "Billboard";
	    if (ai.buildings_list[ai.MyID].Count > 10) {
	        int type_prob = rnd.Next(6);
	        if (type_prob < 3)
	            building_type = "Ornamental";
	        //else if (type_prob < 3)
	        //    building_type = "Entertainment";
	    }
            Vector2Int location = ai.NewDefensiveCoordinates(local_building, building_type);
            if (location[0] != -1)
                ai.CreateBuilding(location, building_type);
	}
	else if (ai.SelectBuildingToBribe()!= null) {
                //call bribe funcion
        }
    }
 }
