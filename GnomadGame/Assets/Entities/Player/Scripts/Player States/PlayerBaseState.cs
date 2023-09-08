using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    protected bool isRootState;
    protected PlayerStateMachine context;
    protected PlayerStateFactory factory;
    protected PlayerBaseState currentSuperState;
    protected PlayerBaseState currentSubState;

    public PlayerBaseState(PlayerStateMachine psm, PlayerStateFactory psf)
    {
        context = psm;
        factory = psf;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        if(currentSubState != null)
        {
            currentSubState.UpdateStates();
        }
    }

    public void FixedUpdateStates()
    {
        FixedUpdateState();
        if(currentSubState != null)
        {
            currentSubState.FixedUpdateStates();
        }
    }
    protected void SwitchState(PlayerBaseState newState) 
    {
        ExitState();
        newState.EnterState();
        if (isRootState)
        {
            context.CurrentState = newState;
        }
        else if(currentSuperState != null)
        {
            currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(PlayerBaseState newSuperState) 
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState) 
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
