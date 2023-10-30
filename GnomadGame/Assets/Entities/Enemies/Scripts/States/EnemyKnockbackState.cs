using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockbackState : EnemyBaseState
{

    float knockbackTimer = 0f;
    Vector3 initialKnockbackDirection = Vector3.zero;
    Vector3 currentKnockbackVelocity = Vector3.zero;
    // add a timer, that when zero, enemy switches back to its normal states

    public EnemyKnockbackState(EnemyStateMachine esm) : base(esm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        // check if timer is done, then if grounded
        if (context.IsGrounded && knockbackTimer <= 0.9f * context.KnockbackTimer)
        {
            SwitchState(context.GroundedState);
            return;
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        InitializeSubState();
        knockbackTimer = context.KnockbackTimer;
        initialKnockbackDirection = context.LastKBDirection;
        if (initialKnockbackDirection.x < 0.5f && initialKnockbackDirection.x > -0.5f)
        {
            initialKnockbackDirection.x = 0.5f * Mathf.Sign(initialKnockbackDirection.x);
        }
        if (initialKnockbackDirection.y < 0.5f && initialKnockbackDirection.y > -0.5f)
        {
            initialKnockbackDirection.y = 0.5f * Mathf.Sign(initialKnockbackDirection.y);
        }
        context.rb.velocity = initialKnockbackDirection * 27.5f;
        currentKnockbackVelocity = context.rb.velocity;

        //context.rb.Sleep();
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
        knockbackTimer = 0f;
        SetSubState(context.MoveState);
        context.IsDamaged = false;
    }

    public override void FixedUpdateState()
    {
        context.rb.velocity = currentKnockbackVelocity + new Vector3(0,  (MovementStats.fallSpeed * Time.fixedDeltaTime));
        currentKnockbackVelocity = context.rb.velocity;
    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();

        SetSubState(null);
    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();

        // get new velocity for knockback

            // get values from MovementStats
            // Check if enemy is on the left or right from collision
            // multiply by time left (or maybe something else)
            // set velocity equal to new velocity

        //Vector2 newVelocity = new(context.KnockbackXConst, context.KnockbackYConst);

        //newVelocity.x *= context.damageDirection;

        //newVelocity = newVelocity.normalized;


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

