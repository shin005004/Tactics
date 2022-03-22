using UnityEngine;

public abstract class ActionBaseState
{
    public abstract void EnterState(ActionManager actionManager);
    public abstract void UpdateState(ActionManager actionManager);
}
