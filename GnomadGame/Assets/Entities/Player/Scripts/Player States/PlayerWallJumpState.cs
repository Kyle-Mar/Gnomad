using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    float startJumpTime;
    float jumpTimer;
    Vector2 initialMovementDir;
    public PlayerWallJumpState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }


    public override void CheckSwitchStates()
    {
        if (context.IsGrounded)
        {
            SwitchState(context.GroundedState);
        }
        if(!context.Controls.Player.Jump.IsPressed() || jumpTimer <= 0)
        {
            SwitchState(context.FallState);
        }
        if (context.Controls.Player.Slide.WasPressedThisFrame())
        {
            SwitchState(context.SlideState);
        }
        if (context.Controls.Player.GroundPound.WasPressedThisFrame())
        {
            SwitchState(context.GroundPoundState);
        }
        
    }

    public override void EnterState()
    {
        InitializeSubState();
        startJumpTime = 0;
        initialMovementDir = context.LastMovementDirection;

        // default direction based on which wall you're touching instead?

        jumpTimer = startJumpTime + MovementStats.maxWallJumpHeight;

        context.SetMoveSpeed(MovementStats.moveSpeedReduced);
        
        context.rb.AddForce(new Vector2(MovementStats.moveSpeed * -initialMovementDir.x, MovementStats.jumpSpeed), ForceMode2D.Impulse);

        Object.Instantiate(context.JumpCloudParticles, context.transform.position, Quaternion.identity);

    }

    public override void ExitState()
    {
        context.SetMoveSpeed(MovementStats.moveSpeed);
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
        CheckSwitchStates();

        jumpTimer -= Time.deltaTime;

        context.SetMoveSpeed( Mathf.Lerp(context.CurrentMoveSpeed, MovementStats.moveSpeed, Utils.GetInterpolant(100f)));

        if (context.Controls.Player.Jump.IsPressed() && jumpTimer > 0)
        {
            context.rb.velocity = new Vector2(MovementStats.moveSpeed * -initialMovementDir.x, MovementStats.jumpSpeed);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }
}
