using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState: BaseState
{
    //I tried using the suggested "new" keyword, but it really doesn't seem to do what we want here. 
    //Feel free to try it yourself. It will stop the errors, but totall screw up the machine
    new protected PlayerStateMachine context;//Trouble
    new protected BaseState currentSuperState;//Trouble
    new protected BaseState currentSubState;//Trouble

    public PlayerBaseState(StateMachine psm) : base(psm)
    {
        context = psm as PlayerStateMachine;
        context.onDamageKB += OnDamageKB;
        currentSubState = base.currentSubState as PlayerBaseState;
        currentSuperState = base.currentSuperState as PlayerBaseState;
    }

    public virtual void OnDamageKB(float amount, Collider2D collision, Vector3 dir)
    {
        if(this != context.CurrentState)
        {
            return;
        }
        Debug.Log("OUCH KB!");
        SwitchState(context.KnockbackState);

    }
}
