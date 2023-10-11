using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used for changing to an empty substate.
public class EnemyMoveState : EnemyBaseState
{


    public EnemyMoveState(EnemyStateMachine esm) : base(esm)
    {
    }


    private void GetMovementVectorToTargetObject()
    {
        Vector2 newVelocity = Vector2.zero;
        float x = context.targetObject.transform.position.x - context.gameObject.transform.position.x;

        if (x < -0.5)
        {
            newVelocity.x = -1 * context.CurrentMoveSpeed;

            /*
            if (CheckIfCollidingWithWall(ref context.col, newVelocity, context.transform, ref context.wallContactFilter))
            {
                //Debug.DrawLine(context.gameObject.transform.position, hitInfo.collider.gameObject.transform.position);

                SwitchState(context.IdleState);
            }
            */

        }
        else if (x > 0.5)
        {
            newVelocity.x = 1 * context.CurrentMoveSpeed;

            /*
            if (CheckIfCollidingWithWall(ref context.col, newVelocity, context.transform, ref context.wallContactFilter))
            {
                //Debug.DrawLine(context.gameObject.transform.position, hitInfo.collider.gameObject.transform.position);
                SwitchState(context.IdleState);
            }
            */
        }

        else
        {
            SwitchState(context.IdleState);
        }



        context.rb.velocity = newVelocity;

        //context.LastMovementDirection = newVelocity.normalized;
        // Could create a movemnt stats script that stores this


    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        // If there is no target object
        if (context.targetObject == null)
        {
            SwitchState(context.IdleState);
        }

        if (context.IsTargetOutOfSight && context.IsAggro)
        {
            SwitchState(context.IdleState);
        }

        bool isGrounded = context.IsGrounded;
        if (isGrounded)
        {
            // If enemy is grounded
            if (context.rb.velocity == Vector2.zero)
            {
                SwitchState(context.IdleState);
            }

            if (context.targetObject)
            {
                // If enemy is close enough to targeted move point, switch to idle
                if (Vector2.Distance(context.gameObject.transform.position, context.targetObject.transform.position) < 0.5f)
                {
                    // if distance is less than this, set velocity to zero
                    // States for Idle and movement will be changed based on velocity
                    SwitchState(context.IdleState);
                }
            }

        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        InitializeSubState();
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

        //SetSubState(context.NotAttackState);
        if (context.IsAttacking)
        {
            SetSubState(context.AttackState);
        }
        else
        {
            SetSubState(context.NotAttackState);
        }
    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();

        // Check if state needs changed

        if (context.targetObject != null)
        {

            // Do movement Stuff here

            if (context.IsAggro)
            {
                // Do Enemy / Player Interaction
                GetMovementVectorToTargetObject();
            }
            else
            {
                // Do Enemy Interaction when not aggro
                GetMovementVectorToTargetObject();
            }
        }
        CheckSwitchStates();

    }

    void Start()
    {

    }
}