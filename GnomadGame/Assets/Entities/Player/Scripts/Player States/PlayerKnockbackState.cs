using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gnomad.Utils;

public class PlayerKnockbackState : PlayerBaseState
{

    float knockbackTimer = 0f;
    float knockbackTime = .9f;
    Vector3 initialKnockbackDirection = Vector3.zero;
    public PlayerKnockbackState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if(context.IsGrounded && knockbackTimer <= 0.9f * knockbackTime)
        {
            SwitchState(context.GroundedState);
            return;
        }
        if(knockbackTimer <= 0f)
        {
            SwitchState(context.FallState);
            //LateInitializeSubState();
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        knockbackTimer = knockbackTime;
        initialKnockbackDirection = context.LastKBDirection.normalized;
        if(initialKnockbackDirection.x < 0.5f && initialKnockbackDirection.x > -0.5f)
        {
            initialKnockbackDirection.x = 0.5f * Mathf.Sign(initialKnockbackDirection.x);
        }
        if (initialKnockbackDirection.y < 0.5f && initialKnockbackDirection.y > -0.5f)
        {
            initialKnockbackDirection.y = 0.5f * Mathf.Sign(initialKnockbackDirection.y);
        }
        Debug.Log("Initial Knockback Direction + " + initialKnockbackDirection);
        context.rb.velocity = initialKnockbackDirection * 27.5f;

    }

    public override void ExitState()
    {
        //Debug.Log("exiting");
        knockbackTimer = 0f;

        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
        context.rb.velocity = new(context.rb.velocity.x, context.rb.velocity.y + (MovementStats.fallSpeed * Time.fixedDeltaTime));
        if (Mathf.Abs(context.rb.velocity.x) <= 10f)
        {
            //context.rb.velocity = Vector3.Lerp(context.rb.velocity, new Vector3(0, MovementStats.fallSpeed), Utils.GetInterpolant(100f));
        }
        else
        {
            //context.rb.velocity = Vector3.Lerp(context.rb.velocity, new Vector3(0, MovementStats.fallSpeed / 4), Utils.GetInterpolant(10f));
        }
        // throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        SetSubState(null);
        //Debug.Log(context.IdleState.CurrentSubState());
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        //Debug.Log(currentSubState);
        /*context.rb.velocity = Vector2.Lerp(context.rb.velocity,
                              new(MovementStats.groundPoundXSpeed, MovementStats.groundPoundYSpeed),
                              Utils.GetInterpolant(100f));*/
        knockbackTimer -= Time.deltaTime;
        CheckSwitchStates();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
