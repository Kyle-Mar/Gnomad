using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    Vector2 initialMovementDir;
    float slideEndTime;
    bool sliding = true;
    public PlayerSlideState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        if (DidBonk())
        {
            context.CheckIfGrounded();
            if (!context.IsGrounded)
            {
                SwitchState(factory.Fall());
            }
            else
            {
                SwitchState(factory.Grounded());
            }
        }else if (!sliding)
        {
            if (!context.IsGrounded)
            {
                SwitchState(factory.Fall());
            }
            else
            {
                SwitchState(factory.Grounded());
            }
        }
    }

    public override void EnterState()
    {
        slideEndTime = Time.time + MovementStats.slideDuration;
        initialMovementDir = context.LastMovementDirection;
        context.rb.velocity = new Vector2(MovementStats.slideSpeedX * initialMovementDir.x, MovementStats.slideFallSpeed);
    }

    public override void ExitState()
    {
        context.rb.velocity = new Vector2(0, 0);
    }

    public override void InitializeSubState()
    {
        SetSubState(factory.Empty());
    }

    public override void UpdateState()
    {
        if(Time.time > slideEndTime)
        {
            sliding = false;
        }
        else
        {
            context.rb.velocity = Vector2.Lerp(context.rb.velocity, new Vector2(0, MovementStats.slideFallSpeed), Utils.GetInterpolant(10f));
        }

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
