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
        //Debug.Log($"{this} + {currentSubState}");
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
        Debug.Log("Switch State" + this + " " + newState);
        ExitState();
        newState.EnterState();
        if (isRootState)
        {
            context.CurrentState = newState;
        }
        else {
            currentSuperState.SetSubState(newState);
        }
       
    }
    protected void SetSuperState(BaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(BaseState newSubState)
    {
        if(newSubState != currentSubState)
        {
            if(newSubState != null)
            {
                newSubState.EnterState();
            }
            if (currentSubState != null)
            {
                currentSubState.ExitState();
            }
        }
        currentSubState = newSubState;
        //Debug.Log($"{newSubState} {currentSuperState}");
        
        if(newSubState != null)
        {
            newSubState.SetSuperState(this);
        }
    }
    public string CurrentSubState()
    {
        return currentSubState.ToString();
    }
}
