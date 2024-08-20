using UnityEngine;

//Action/State         office	Def	Off	Wait	Bribery
//1. $/sec < 10		60%	15%	15%	10%	0%
//2. $/sec < 20		40%	20%	30%	10%	0%
//3. $/sec < 40		20%	20%	50%	10%	0%
//4. $/sec < 80		10%	20%	60%	10%	0%
//5. $/sec < 160	0%	29%	70%	0%	1%
//6. others		0%	18%	80%	0%	2%


public class AIWarState : AIBaseState 
{
    float[] IncomeLevel = new float[5]{10, 20, 40,80, 160};
    int[,] BuildChoice = new int[6,5]  {{60, 75, 90, 100, 100},
		    			{40, 60, 90, 100, 100},
		    			{20, 40, 90, 100, 100},
    				   	{10, 30, 90, 100, 100},
    				  	{ 0, 27, 97,  97, 100},
    					{ 0, 15, 95,  95, 100}};

    public override void EnterState(AIManager ai) 
    {
    }
    public override void UpdateState(AIManager ai) 
    {
        if (ai.InPanic) 
            ai.SwitchState(new AIPanicState());
        else if (ai.IsBerzerk) 
            ai.SwitchState(new AIBerzerkState());
    }
    public override void SelectAction(AIManager ai)
    {
        System.Random rnd = new System.Random();
        int prob = rnd.Next(100);
        int income_index = 0;
        float current_income = LevelManager.Instance.Incomes[ai.MyID];
        while ((income_index < IncomeLevel.Length) && (current_income > IncomeLevel[income_index]))
            income_index++;
	int prob_index = 0;
	while ((prob_index < BuildChoice.Length - 1) && (prob > BuildChoice[income_index, prob_index]))
            prob_index++;
        Building local_building;
        string building_type;
        switch (prob_index)
        {
        case 0:
            local_building = ai.SelectDefensiveBuilding();
            Vector2Int location = ai.NewOfficeCoordinates(local_building);
            if (location[0] != -1)
                ai.CreateBuilding(location, "Office");
            break;
        case 1:
	    local_building = ai.SelectDefensiveBuilding();
	    building_type = "Billboard";
	    if (ai.buildings_list[ai.MyID].Count > 10) {
	        int type_prob = rnd.Next(6);
	        if (type_prob < 2)
	            building_type = "Ornamental";
	        else if (type_prob < 3)
	            building_type = "Entertainment";
	    }
            location = ai.NewDefensiveCoordinates(local_building, building_type);
            if (location[0] != -1)
                ai.CreateBuilding(location, building_type);
            break;        
        case 2:
	    local_building = ai.SelectOffensiveBuilding();
	    building_type = "Billboard";
	    if (ai.buildings_list[ai.MyID].Count > 10) {
	        int type_prob = rnd.Next(6);
	        if (type_prob < 2)
	            building_type = "Ornamental";
	        else if (type_prob < 3)
	            building_type = "Entertainment";
	    }
            location = ai.NewOffensiveCoordinates(local_building, building_type);
            if (location[0] != -1) 
                ai.CreateBuilding(location, building_type);
            break;        
        case 3:
            // Wait
            break;
        default:
            if (ai.SelectBuildingToBribe()!= null) {
                //call bribe funcion
            }
            break;
	}
    }
}
