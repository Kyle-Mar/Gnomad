using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used for changing to an empty substate.
public class EnemyMoveState : EnemyBaseState
{

    Vector2 currentMoveDirection = Vector2.zero;
    Vector2 lastPosition = Vector2.zero;

    public EnemyMoveState(EnemyStateMachine esm) : base(esm)
    {
    }


    private void GetMovementVectorToTargetObject()
    {
        Vector2 newVelocity = Vector2.zero;
        float x = context.targetObject.transform.position.x - context.gameObject.transform.position.x;
        if (!context.IsDamaged)
        {

            if (context.IsGrounded)
            {

                if (!context.IsAttacking)
                {
                    if (x < -0.5)
                    {
                        newVelocity.x = -1 * context.CurrentMoveSpeed;
                        currentMoveDirection = newVelocity.normalized;
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
                        currentMoveDirection = newVelocity.normalized;

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
                        currentMoveDirection = Vector2.zero;
                        SwitchState(context.IdleState);
                    }
                }

                else
                {
                    // Charging Logic

                    newVelocity = currentMoveDirection * context.CurrentMoveSpeed;

                    // In case it is just running into a wall
                    if (Vector2.Distance(lastPosition, context.transform.position) < 0.001f)
                    {
                        SetSubState(context.NotAttackState);
                        SwitchState(context.IdleState);
                    }

                    lastPosition = context.transform.position;

                }
            }
            else
            {
                context.rb.velocity = new Vector2((currentMoveDirection * context.CurrentMoveSpeed).x,context.rb.velocity.y);
            }
        }



        context.rb.velocity = new Vector2(newVelocity.x, context.rb.velocity.y);

        //context.LastMovementDirection = newVelocity.normalized;
        // Could create a movemnt stats script that stores this


    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        // If there is no target object
        if (context.targetObject == null || context.JustAttacked)
        {
            context.JustAttacked = false;
            SwitchState(context.IdleState);
        }

        if (context.IsTargetOutOfSight && context.IsAggro && !context.IsAttacking)
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
        else
        {
            //SwitchState(context.IdleState);
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();

        // Setting currentMoveDirection here so if the enemy starts attacking
        // in Idle state, it will be gaurenteed to be charging in the correct direction
        currentMoveDirection = new Vector2(context.targetObject.transform.position.x - context.gameObject.transform.position.x, 0).normalized;
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
        if (context.IsAttacking && !context.IsDamaged)
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