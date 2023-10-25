using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockbackState : EnemyBaseState
{

    float knockbackTimer = 0f;

    // add a timer, that when zero, enemy switches back to its normal states

    public EnemyKnockbackState(EnemyStateMachine esm) : base(esm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        // check if timer is done, then if grounded
        if (knockbackTimer <= 0f)
        {
            context.CheckIfGrounded();
            if (context.IsGrounded)
            {
                SwitchState(context.GroundedState);
            }
            else
            {
                SwitchState(context.FallState);
            }
        }

    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        knockbackTimer = context.KnockbackTimer;
        InitializeSubState();
        context.rb.velocity = Vector2.zero;
        SetSubState(null);
        //context.rb.Sleep();
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
        SetSubState(context.MoveState);
        context.IsDamaged = false;
    }

    public override void FixedUpdateState()
    {
        // throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();

        // Initialize Idle State
        SetSubState(context.EmptyState);
    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();

        // get new velocity for knockback

            // get values from MovementStats
            // Check if enemy is on the left or right from collision
            // multiply by time left (or maybe something else)
            // set velocity equal to new velocity

        Vector2 newVelocity = new(context.KnockbackXConst, context.KnockbackYConst);

        newVelocity.x *= context.damageDirection;

        //newVelocity = newVelocity.normalized;

        context.rb.AddForce(newVelocity, ForceMode2D.Impulse);

        // apply gravity
        //Physics2D.gravity = new(0, context.FallSpeed);

        // Update timer
        knockbackTimer -= Time.deltaTime;

        // Check Switch States
        CheckSwitchStates();
    }

    void Start()
    {

    }
}

