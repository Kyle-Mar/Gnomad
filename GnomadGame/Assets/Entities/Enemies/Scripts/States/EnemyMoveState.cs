using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used for changing to an empty substate.
public class EnemyMoveState : EnemyBaseState
{
    public EnemyMoveState(EnemyStateMachine esm) : base(esm)
    {
    }
    
    private Vector2 GetMovementVectorToTargetObject()
    {
        Vector2 newVelocity = Vector2.zero;
        newVelocity.x = context.targetObject.transform.position.x - context.gameObject.transform.position.x;
        newVelocity.Normalize();

        // Could create a movemnt stats script that stores this
        newVelocity *= 5;

        return newVelocity;
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        bool isGrounded = context.IsGrounded;
        if (isGrounded)
        {
            // If enemy is grounded
            
            //  And is not Aggro

            if (!context.IsAggro)
            {
                // If enemy is close enough to targeted move point, switch to idle
                if (context.targetObject == null) 
                { 
                    context.targetObject = context.movePoints[context.currentMovePointIndex].gameObject;
                }

                if (Vector2.Distance(context.gameObject.transform.position, context.targetObject.transform.position) < 0.4f)
                {
                    // if distance is less than this, set velocity to zero
                    // States for Idle and movement will be changed based on velocity
                    SwitchState(context.IdleState);
                }
            }
            
            //  And is Aggro
            
            else
            {
                float dist = Vector2.Distance(context.gameObject.transform.position, context.targetObject.transform.position);
                //Debug.Log(dist);

                // If the enemy is close enough
                //  and the enemy's attack isn't on cooldown
                if (dist < 2f && !context.IsAttackOnCooldown)
                {
                    //Debug.Log("Entering Attack");
                    if (currentSubState != context.AttackState)
                    {
                        SetSubState(context.AttackState);
                    }
                }
                else if (!context.IsAttacking)
                {
                    Debug.Log("Enemy Is Not Attacking");
                    SetSubState(context.NotAttackState);
                }
            }
        }
        else
        {

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
                context.rb.velocity = GetMovementVectorToTargetObject();
            }
            else
            {
                // Do Enemy Interaction when not aggro
                context.rb.velocity = GetMovementVectorToTargetObject();
            }
        }

        else
        {
            SwitchState(context.IdleState);
        }
        CheckSwitchStates();
    }

    void Start()
    {

    }
}

