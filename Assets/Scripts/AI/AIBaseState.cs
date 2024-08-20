using UnityEngine;

public abstract class AIBaseState 
{
    public abstract void EnterState(AIManager ai);
    public abstract void UpdateState(AIManager ai);
    public abstract void SelectAction(AIManager ai);
}
