using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for changing to an empty substate.
public class PlayerEmptyState : PlayerBaseState
{
    public PlayerEmptyState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
       // throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        SetSubState(null);
    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
