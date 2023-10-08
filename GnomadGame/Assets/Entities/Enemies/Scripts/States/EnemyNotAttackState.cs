using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for changing to an empty substate.
public class EnemyNotAttackState : EnemyBaseState
{
    public EnemyNotAttackState(EnemyStateMachine esm) : base(esm)
    {
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();
        if (context.IsAttacking)
        {
            if (!context.IsAttackOnCooldown)
            {
                SwitchState(context.AttackState);
            }
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        context.IsAttacking = false;

        CheckSwitchStates();
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
        CheckSwitchStates();
    }

    void Start()
    {

    }
}
