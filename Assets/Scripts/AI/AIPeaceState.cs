using UnityEngine;

//Action/State         office	Def	Off	Wait	Bribery
//1. $/sec < 10		80%	0%	0%	20%	0%
//2. $/sec < 20		50%	0%	30%	20%	0%
//3. $/sec < 40		30%	10%	40%	20%	0%
//4. $/sec < 80		20%	30%	40%	10%	0%
//5. $/sec < 160	10%	30%	50%	10%	0%
//6. others		0%	30%	70%	0%	0%

public class AIPeaceState : AIBaseState 
{
    float[] IncomeLevel = new float[5]{10, 20, 40,80, 160};
    int[,] BuildChoice = new int[6,5]  {{80, 80, 80, 100, 100},
		    			{50, 50, 80, 100, 100},
		    			{30, 40, 80, 100, 100},
    				   	{20, 50, 90, 100, 100},
    				  	{10, 40, 90, 100, 100},
    					{0, 30, 100, 100, 100}};
    public override void EnterState(AIManager ai) {}

    public override void UpdateState(AIManager ai) 
    {
        if (ai.InPanic) 
            ai.SwitchState(new AIPanicState());
        else if (ai.IsBerzerk)
            ai.SwitchState(new AIBerzerkState());
        else if (ai.WarStarted)
            ai.SwitchState(new AIWarState());
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
            if (location[0] != -1) {
                // Precisa fazer o build information de office e verificar os parâmetros de Cells
                LevelManager.Instance.ConstructBuilding(ai.MyID, LevelManager.Instance.GridController.Cells[location[0],location[1]], 
                					new BuildingInformation());
            }
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
            if (location[0] != -1) {
                // Precisa fazer o build information de office e verificar os parâmetros de Cells
                LevelManager.Instance.ConstructBuilding(ai.MyID, LevelManager.Instance.GridController.Cells[location[0],location[1]], 
                					new BuildingInformation());
            }
            break;        
        case 2:
            // While in peace, build Billboards for offensive power.
	    local_building = ai.SelectOffensiveBuilding();
	    building_type = "Billboard";
            location = ai.NewExpansionCoordinates(local_building, building_type);
            if (location[0] != -1) {
                // Precisa fazer o build information de office e verificar os parâmetros de Cells
                LevelManager.Instance.ConstructBuilding(ai.MyID, LevelManager.Instance.GridController.Cells[location[0],location[1]], 
                					new BuildingInformation());
            }
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
        //ai.SelectDefensiveBuilding();
        //ai.SelectOffensiveBuilding();
    }
}
