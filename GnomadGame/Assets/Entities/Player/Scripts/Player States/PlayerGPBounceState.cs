using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerGPBounceState : PlayerBaseState
{
    public PlayerGPBounceState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    float startBounceTime;
    float maxBounceTime;

    public override void CheckSwitchStates()
    {
        if (context.DoWallSlide().Item2)
        {
            //SwitchState(factory.WallSlide());
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
        if(maxBounceTime < 0)
        {
            SwitchState(context.FallState);
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.IsGPBounceQueued = false;
        Debug.Log("GroundPoundBounce state has been entered");
        startBounceTime = 0;
        maxBounceTime = startBounceTime + MovementStats.maxGPBounceHeight;
        context.rb.velocity = new Vector2(context.rb.velocity.x, MovementStats.bounceSpeed);
        context.Animator.SetBool("IsJumping", true);
    }

    public override void ExitState()
    {
        context.Animator.SetBool("IsJumping", false);
        //context.rb.velocity = new(context.rb.velocity.x, context.rb.velocity.y / 3);
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

        maxBounceTime -= Time.deltaTime;

        if (maxBounceTime > 0)
        {
            context.rb.velocity = new(context.rb.velocity.x, MovementStats.bounceSpeed);
        }
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }
}
