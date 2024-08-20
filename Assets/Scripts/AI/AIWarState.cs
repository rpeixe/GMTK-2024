using UnityEngine;

//Action/State         office	Def	Off	Wait	Bribery
//1. $/sec < 10		60%	15%	15%	10%	0%
//2. $/sec < 20		40%	0%	30%	10%	0%
//3. $/sec < 40		20%	20%	50%	10%	0%
//4. $/sec < 80		10%	20%	60%	10%	0%
//5. $/sec < 160	0%	29%	70%	0%	1%
//6. others		0%	18%	80%	0%	2%


public class AIWarState : AIBaseState 
{
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
    	//return(ai.SelectDefensiveBuilding());
    	//return(ai.SelectOffensiveBuilding());
    }
}
