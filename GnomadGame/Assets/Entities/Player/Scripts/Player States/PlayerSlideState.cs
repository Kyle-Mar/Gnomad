using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    Vector2 initialMovementDir;
    float slideEndTime;
    bool sliding = true;
    public PlayerSlideState(PlayerStateMachine psm) : base(psm)
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
        }else if (!sliding)
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
        sliding = true;
        slideEndTime = Time.time + MovementStats.slideDuration;
        initialMovementDir = context.LastMovementDirection;
        context.rb.velocity = new Vector2(MovementStats.slideSpeedX * initialMovementDir.x, MovementStats.fallSpeed/5);
    }

    public override void ExitState()
    {
        context.rb.velocity = new Vector2(0, 0);
    }

    public override void FixedUpdateState()
    {
        //Debug.Log("Time.Time: " + Time.time + " slideEndTime " + slideEndTime);
        if (Time.time > slideEndTime)
        {
            sliding = false;
        }
        else
        {
            context.rb.velocity = new Vector2(MovementStats.slideSpeedX * initialMovementDir.x, MovementStats.slideFallSpeed);
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
