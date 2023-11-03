using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerBaseState
{
    public PlayerWallSlideState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    float timer;

    public override void CheckSwitchStates()
    {
        float inputX = context.Controls.Player.Move.ReadValue<Vector2>().x;
        if (context.IsGrounded)
        {
            SwitchState(context.GroundedState);
        }
        var wallSlide = context.DoWallSlide();
        if ((context.Controls.Player.Jump.WasPressedThisFrame() || context.JumpBufferTime > 0) && wallSlide.Item1 != Vector2.zero)
        {
            context.ConsumeJumpBuffer();
            context.WallJumpState.SetJumpDirection(wallSlide.Item1);
            SwitchState(context.WallJumpState);
        }
        if (context.Controls.Player.Dash.WasPressedThisFrame())
        {
            SwitchState(context.DashState);
        }
        if (timer <= 0)
        {
            context.SetWallSlideExpired(true);
            SwitchState(context.FallState);
        }
        if (context.IsTouchingWallLeft && inputX >= 0)
        {
            SwitchState(context.FallState);
        }
        if(context.IsTouchingWallRight && inputX <= 0)
        {
            SwitchState(context.FallState);
        }
        if (!context.IsTouchingWall)
        {
            SwitchState(context.FallState);
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        timer = 0.5f;
        context.Animator.SetBool("IsWallSlide", true);
    }


    public override void ExitState()
    {
        context.SetMoveSpeed(MovementStats.moveSpeed);
        context.Animator.SetBool("IsWallSlide", false);
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        SetSubState(context.EmptyState);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Physics2D.gravity = new(0, 0);
        context.rb.velocity = new(0, MovementStats.wallSlideSpeed);
        timer -= Time.deltaTime;

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
