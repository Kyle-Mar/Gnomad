using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
        isRootState = true;
        InitializeSubState();
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
            SwitchState(factory.Slide());
        }
        if (context.Controls.Player.GroundPound.WasPressedThisFrame())
        {
            SwitchState(factory.GroundPound());
        }
    }

    public override void EnterState()
    {
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
            SetSubState(factory.Run());
        }
        else
        {
            SetSubState(factory.Idle());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        maxJumpTime -= Time.deltaTime;

        if (context.Controls.Player.Jump.IsPressed() && maxJumpTime > 0)
        {
            context.rb.velocity = new(context.rb.velocity.x, MovementStats.jumpSpeed);
        }
        else
        {
            //Debug.Log(context.transform.position.y);
            SwitchState(factory.Fall());
        }
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }
}
