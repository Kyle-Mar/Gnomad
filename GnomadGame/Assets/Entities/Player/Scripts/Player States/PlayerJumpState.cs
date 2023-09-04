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
        Debug.Log("HELLO I AM JUMPING");
        startJumpTime = Time.time;
        maxJumpTime = startJumpTime + MovementStats.maxJumpHeight;
        context.rb.velocity = new Vector2(context.rb.velocity.x, MovementStats.jumpSpeed);
        //throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        //
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

        if (context.Controls.Player.Jump.IsPressed() && Time.time < maxJumpTime)
        {
            context.rb.AddForce(Vector2.up * MovementStats.jumpSpeed);
        }
        else
        {
            SwitchState(factory.Fall());
        }
    }
}
