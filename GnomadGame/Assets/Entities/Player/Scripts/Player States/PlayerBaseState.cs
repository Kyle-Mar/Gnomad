using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState: BaseState
{
    //I tried using the suggested "new" keyword, but it really doesn't seem to do what we want here. 
    //Feel free to try it yourself. It will stop the errors, but totall screw up the machine
    protected PlayerStateMachine context;//Trouble
    protected PlayerBaseState currentSuperState;//Trouble
    protected PlayerBaseState currentSubState;//Trouble

    public PlayerBaseState(PlayerStateMachine psm) : base(psm)
    {
        context = (PlayerStateMachine)psm;
        currentSubState = base.currentSubState as PlayerBaseState;
        currentSuperState = base.currentSuperState as PlayerBaseState;
    }
}
