using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for changing to an empty substate.
public class PlayerEmptyState : PlayerBaseState
{
    public PlayerEmptyState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
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
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
