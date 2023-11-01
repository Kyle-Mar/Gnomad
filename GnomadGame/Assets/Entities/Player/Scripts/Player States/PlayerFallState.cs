using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if (context.InCoyoteRange==true && context.Controls.Player.Jump.WasPressedThisFrame())
        {
            context.ConsumeJumpBuffer();
            SwitchState(context.JumpState);
            return;
        }
        if (context.IsGrounded)
        {
            SwitchState(context.GroundedState);
            return;
        }
        var wallSlide = context.DoWallSlide();
        if(wallSlide.Item2 && context.JumpBufferTime > 0f && wallSlide.Item1 != Vector2.zero)
        {
            context.WallJumpState.SetJumpDirection(wallSlide.Item1);
            SwitchState(context.WallJumpState);
            context.ConsumeJumpBuffer();
            return;
        }
        if (wallSlide.Item2)
        {
            SwitchState(context.WallSlideState);
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

    }

    public override void EnterState()
    {
        //Debug.Log("HELLO I AM FALLING");
        //throw new System.NotImplementedException();
        InitializeSubState();
        context.Animator.SetBool("IsFalling", true);
    }

    public override void ExitState()
    {
        Physics2D.gravity = new(0, -9.8f);
        //throw new System.NotImplementedException();
        context.Animator.SetBool("IsFalling", false);

    }

    public override void FixedUpdateState()
    {
        context.rb.velocity = new(context.rb.velocity.x, context.rb.velocity.y + (MovementStats.fallSpeed * Time.fixedDeltaTime));
        //throw new System.NotImplementedException();
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
        //Debug.Log("HELLO WORLD");

        
        //Physics2D.gravity = new(0, MovementStats.fallSpeed);
        //clamp fall speed
        context.rb.velocity = new Vector2(context.rb.velocity.x,
            Mathf.Max(context.rb.velocity.y, -MovementStats.maxFallSpeed));

        CheckSwitchStates();

        //context.rb.velocity = Vector2.Lerp(context.rb.velocity, new Vector2(context.rb.velocity.x, MovementStats.fallSpeed), Utils.GetInterpolant(10f));
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
