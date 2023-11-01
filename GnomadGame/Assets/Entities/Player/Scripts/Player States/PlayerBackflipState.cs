using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBackflipState : PlayerBaseState
{
    public PlayerBackflipState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    float backflipEndTime;

    public override void CheckSwitchStates()
    {
        //allows the player to wall backflip while assending
        if (context.DoWallSlide().Item2 && 
            (context.JumpBufferTime > 0 || context.Controls.Player.Jump.WasPressedThisFrame()))
        {

            context.ConsumeJumpBuffer();
            SwitchState(context.JumpState);
            return;
        }
        if (context.Controls.Player.Dash.WasPressedThisFrame() && context.CanDash)
        {
            SwitchState(context.DashState);
            return;
        }
        if (context.Controls.Player.Slide.WasPressedThisFrame() && context.CanSlide)
        {
            SwitchState(context.SlideState);
            return;
        }
        if (context.Controls.Player.GroundPound.WasPressedThisFrame())
        {
            SwitchState(context.GroundPoundState);
            return;
        }
        if(Time.time > backflipEndTime)
        {
            SwitchState(context.FallState);
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        backflipEndTime = Time.time + MovementStats.maxBackflipHeight;
        context.rb.velocity = new(-context.LastMovementDirection.x * MovementStats.backflipHorizontalSpeed, 
            MovementStats.backflipVerticalSpeed);
        Object.Instantiate(context.JumpCloudParticles, context.Feet.position, Quaternion.identity);

    }

    public override void ExitState()
    {
         context.rb.velocity = new(context.rb.velocity.x, context.rb.velocity.y / 3);
    }

    public override void InitializeSubState()
    {
        if (context.Controls.Player.Move.ReadValue<Vector2>().x != 0)
        {
            SetSubState(context.RunState);
        }
        else
        {
            SetSubState(context.IdleState);
        }
    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
        //context.rb.velocity = new(-context.LastMovementDirection.x * MovementStats.backflipHorizontalSpeed,
        //    MovementStats.backflipVerticalSpeed);
        context.rb.velocity = new(context.rb.velocity.x, context.rb.velocity.y + (MovementStats.fallSpeed * Time.fixedDeltaTime));

        CheckSwitchStates();
    }
}
