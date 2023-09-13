using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected bool isRootState;
    protected StateMachine context;
    protected BaseState currentSuperState;
    protected BaseState currentSubState;

    public BaseState(StateMachine sm)
    {
        context = sm;
    }

    public abstract void EnterState();
    public abstract void UpdateState();

    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (currentSubState != null)
        {
            currentSubState.UpdateStates();
        }
    }

    public void FixedUpdateStates()
    {
        FixedUpdateState();
        if (currentSubState != null)
        {
            currentSubState.FixedUpdateStates();
        }
    }
    protected void SwitchState(BaseState newState)
    {
        ExitState();
        newState.EnterState();
        if (isRootState)
        {

            context.CurrentState = newState;
        }
        else if (currentSuperState != null)
        {
            currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(BaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(BaseState newSubState)
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
