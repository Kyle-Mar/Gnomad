using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    Vector2 initialMovementDir;
    float dashEndTime;
    bool dashing = true;

    public PlayerDashState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if (DidBonk())
        {
            context.CheckIfGrounded();
            if (!context.IsGrounded)
            {
                SwitchState(context.FallState);
            }
            else
            {
                SwitchState(context.GroundedState);
            }
        }else if (!dashing)
        {
            if (!context.IsGrounded)
            {
                SwitchState(context.FallState);
            }
            else
            {
                SwitchState(context.GroundedState);
            }
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        initialMovementDir = context.LastMovementDirection;
        dashing = true;
        context.Animator.SetTrigger("DashTrigger");
        context.Animator.SetBool("Dashing", true);


        dashEndTime = Time.time + MovementStats.dashDuration;
        context.rb.velocity = new Vector2(MovementStats.dashSpeed * initialMovementDir.x, 0);
    }

    public override void ExitState()
    {
        context.rb.velocity = new Vector2(0, 0);
        context.Animator.SetBool("Dashing", false);

    }

    public override void FixedUpdateState()
    {
        //Debug.Log("Time.Time: " + Time.time + " dashEndTime " + dashEndTime);
        if (Time.time > dashEndTime)
        {
            dashing = false;
        }
        else
        {
            context.rb.velocity = new Vector2(MovementStats.dashSpeed * initialMovementDir.x, 0);
        }
    }

    public override void InitializeSubState()
    {
        SetSubState(context.EmptyState);
    }

    public override void UpdateState()
    {

        CheckSwitchStates();
    }

    bool DidBonk()
    {
        return (context.IsTouchingWallRight && initialMovementDir.x > 0
            || context.IsTouchingWallLeft && initialMovementDir.x < 0);
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
