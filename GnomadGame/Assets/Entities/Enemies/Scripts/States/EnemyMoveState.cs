using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used for changing to an empty substate.
public class EnemyMoveState : EnemyBaseState
{
    public EnemyMoveState(EnemyStateMachine esm) : base(esm)
    {
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();
        if (Vector2.Distance(context.targetObject.transform.position, context.gameObject.transform.position) <= 0.5f)
        {
            SwitchState(context.IdleState);
        }
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

        // Check if state needs changed
        CheckSwitchStates();

        if (context.targetObject != null)
        {
            bool isGrounded = context.IsGrounded;
            if (isGrounded)
            {
                // Do movement Stuff here
                
                Vector2 newVelocity = Vector2.zero;
                newVelocity.x = context.targetObject.transform.position.x - context.gameObject.transform.position.x;
                newVelocity.Normalize();
                newVelocity *= 5;
                context.rb.velocity = newVelocity;
            }
            else
            {
                // Check if falling or jumping here
            }
        }

        else
        {
            SwitchState(context.EmptyState);
        }
    }

    void Start()
    {

    }
}

