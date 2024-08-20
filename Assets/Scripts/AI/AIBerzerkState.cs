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
        //return(ai.SelectOffensiveBuilding());
    }
}
