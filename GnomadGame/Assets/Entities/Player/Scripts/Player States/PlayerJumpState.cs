using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    float startJumpTime;
    float maxJumpTime;

    public override void CheckSwitchStates()
    {
        if (context.DoWallSlide())
        {
            //SwitchState(factory.WallSlide());
        }

        if (context.Controls.Player.Slide.WasPressedThisFrame())
        {
            SwitchState(context.SlideState);
            return;
        }
        if (context.Controls.Player.GroundPound.WasPressedThisFrame())
        {
            SwitchState(context.GroundPoundState);
            return;
        }
        if(maxJumpTime < 0 || !context.Controls.Player.Jump.IsPressed())
        {
            SwitchState(context.FallState);
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        startJumpTime = 0;
        maxJumpTime = startJumpTime + MovementStats.maxJumpHeight;
        context.rb.velocity = new Vector2(context.rb.velocity.x, MovementStats.jumpSpeed);
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

        maxJumpTime -= Time.deltaTime;

        if (context.Controls.Player.Jump.IsPressed() && maxJumpTime > 0)
        {
            context.rb.velocity = new(context.rb.velocity.x, MovementStats.jumpSpeed);
        }
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }
}
