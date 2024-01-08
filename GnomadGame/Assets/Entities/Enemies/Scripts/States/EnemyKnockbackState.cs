using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockbackState : EnemyBaseState
{

    float knockbackTimer = 0f;
    Vector3 initialKnockbackDirection = Vector3.zero;
    Vector3 currentKnockbackVelocity = Vector3.zero;
    float knockbackSpeed = 0f;
    // add a timer, that when zero, enemy switches back to its normal states

    public EnemyKnockbackState(EnemyStateMachine esm) : base(esm)
    {
        isRootState = true;
    }


    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        // check if timer is done, then if grounded
        if (context.IsOnCeiling)
        {
            SwitchState(context.FallState);
        }
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
        //context.animator.SetTrigger("InAirTrigger");
        context.animator.SetBool("InAir", true);
        knockbackTimer = context.KnockbackTimer;
        initialKnockbackDirection = context.knockableComp.KBDirection;

        //Debug.Log("IsVolleyedInto = " + context.IsVolleyedInto);

        

        // For when an enemy hits another enemy

        if (context.IsVolleyedInto)
        {
            if (context.IsGrounded)
            {
                initialKnockbackDirection.y = 0.5f;
            }
            if (context.IsVolleyed)
            {
                // should be multiplied by 12.5f
                context.rb.velocity = initialKnockbackDirection * 12.5f;

                //Debug.Log("Normal Volley knockback");
            }
            else
            {
                // should be multiplied by 32.5f
                context.rb.velocity = initialKnockbackDirection * 32.5f;
                //Debug.Log("Not Volley");
            }
        }

        // If a player hits an enemy

        else if (context.IsGrounded)
        {
            if (context.IsSlidedInto)
            {
                // should be multiplied by 27.5f
                context.rb.velocity = initialKnockbackDirection * 20.5f;
            }
            else
            {

                if (initialKnockbackDirection.x < 0.5f && initialKnockbackDirection.x > -0.5f)
                {
                    initialKnockbackDirection.x = 0.5f * Mathf.Sign(initialKnockbackDirection.x);
                }
                if (initialKnockbackDirection.y < 0.5f && initialKnockbackDirection.y > -0.5f)
                {
                    initialKnockbackDirection.y = 0.5f * Mathf.Sign(initialKnockbackDirection.y);
                }

                // should be multiplied by 12.5f
                context.rb.velocity = initialKnockbackDirection * 12.5f;
            }
        }
        // Knockback for when player volleys enemy
        else
        {
            // Since the KB.y value is higher on the slide, if multiplied by 42.5,
            // It will actually give have an insane amount of y knockback
            // So that's why there's a check here
            //Debug.LogWarning(context.IsSlidedInto);
            if (context.IsSlidedInto)
            {
                context.rb.velocity = initialKnockbackDirection * 20.5f;
            }
            else
            {
                context.rb.velocity = initialKnockbackDirection * 42.5f;
            }
        }
        
        //context.rb.velocity = initialKnockbackDirection * 27.5f;
        currentKnockbackVelocity = context.rb.velocity;
        //Debug.Log(currentKnockbackVelocity);

        //context.rb.Sleep();
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
        //context.animator.SetBool("InAir", false);
        knockbackTimer = 0f;
        context.IsDamaged = false;
        context.IsSlidedInto = false;
        context.IsVolleyed = false;
        context.volleyCol.gameObject.SetActive(false);
        SetSubState(context.MoveState);
        
    }

    public override void FixedUpdateState()
    {
        if (!context.IsSlidedInto)
        {
            context.rb.velocity = currentKnockbackVelocity + new Vector3(0,  (MovementStats.fallSpeed * Time.fixedDeltaTime));
        }
        else
        {
            context.rb.velocity = currentKnockbackVelocity + new Vector3(0,  (MovementStats.fallSpeed * Time.fixedDeltaTime * 0.42f));
        }
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

