using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : BaseState
{
    new protected EnemyStateMachine context;
    new protected BaseState currentSuperState;
    new protected BaseState currentSubState;

    public EnemyBaseState(StateMachine esm) : base(esm)
    {
        context = esm as EnemyStateMachine;
        context.onDamageKB += OnDamageKB;
        currentSubState = base.currentSubState as EnemyBaseState;
        currentSuperState = base.currentSuperState as EnemyBaseState;
    }
    public virtual void OnDamageKB(float amount, Collider2D collider, Vector3 dir)
    {
        if (this != context.CurrentState)
        {
            return;
        }
        SwitchState(context.KnockbackState);
    }
}
