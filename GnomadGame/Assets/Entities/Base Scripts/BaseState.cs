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
        //Debug.Log("Switch State" + this + " " + newState);
        ExitState();
        newState.EnterState();
        if (isRootState)
        {
            if(context.CurrentState.currentSubState != null)
            {
                newState.SetSubState(context.CurrentState.currentSubState);
            }
            context.CurrentState = newState;
        }
        else {
            //newState.SetSuperState(currentSuperState);
            //Debug.Log(currentSuperState.ToString() + this.ToString());
            if(currentSuperState.currentSubState == null)
            {
                return;
            }
            currentSuperState.SetSubState(newState);
        }
       
    }
    protected void SetSuperState(BaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(BaseState newSubState)
    {
        if (newSubState == null) 
        {
            Debug.Log("I AM NULL" + $"{this}");
        }
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
        if(newSubState == null && currentSubState != null)
        {
            currentSubState.SetSuperState(null);
        }
        currentSubState = newSubState;


        if (newSubState != null)
        {
            newSubState.SetSuperState(this);
        }
    }
    public string CurrentSubState()
    {
        return currentSubState.ToString();
    }

    public void PrintStateTree()
    {
        string output = this.ToString();
        output += " -> ";
        BaseState nextStateDown = currentSubState;
        while(nextStateDown!=null)
        {
            output += nextStateDown.ToString();
            if(nextStateDown.currentSubState != null)
            {
                //output += nextStateDown.currentSuperState.ToString();
                output += " -> ";
            }
            nextStateDown = nextStateDown.currentSubState;
        }
        //Debug.Log(output);
    }
}
