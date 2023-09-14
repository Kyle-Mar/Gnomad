using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState: BaseState
{
    //I tried using the suggested "new" keyword, but it really doesn't seem to do what we want here. 
    //Feel free to try it yourself. It will stop the errors, but totall screw up the machine
    protected PlayerStateMachine context;//Trouble
    protected BaseState currentSuperState;//Trouble
    protected BaseState currentSubState;//Trouble

    public PlayerBaseState(StateMachine psm) : base(psm)
    {
        context = psm as PlayerStateMachine;
        currentSubState = base.currentSubState as PlayerBaseState;
        currentSuperState = base.currentSuperState as PlayerBaseState;
    }
}
