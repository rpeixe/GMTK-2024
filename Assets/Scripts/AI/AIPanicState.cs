using UnityEngine;

////Action/State        office	Def	Off	Wait	Bribery
//-			20%	80%	0%	0%	0%	(upgrade in range or buy another one close)

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
        //return(ai.SelectDefensiveBuilding());
    }
}
