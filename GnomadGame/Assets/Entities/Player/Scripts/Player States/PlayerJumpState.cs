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
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchState(factory.Slide());
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
        if (Input.GetAxis("Horizontal") != 0)
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

        if (Input.GetKey(KeyCode.Space) && Time.time < maxJumpTime)
        {
            context.rb.AddForce(Vector2.up * MovementStats.jumpSpeed);
        }
        else
        {
            SwitchState(factory.Fall());
        }
    }
}
