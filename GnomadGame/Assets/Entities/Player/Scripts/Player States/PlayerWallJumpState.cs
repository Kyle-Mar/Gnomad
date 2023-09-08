using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
        isRootState = true;
        InitializeSubState();
    }

    float startJumpTime;
    float maxJumpTime;
    Vector2 initialMovementDir;

    public override void CheckSwitchStates()
    {
        if (context.IsGrounded)
        {
            SwitchState(factory.Grounded());
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
        initialMovementDir = context.LastMovementDirection;
        maxJumpTime = startJumpTime + MovementStats.maxJumpHeight/5;

        context.SetMoveSpeed(MovementStats.moveSpeedReduced);
        
        context.rb.AddForce(new Vector2(MovementStats.moveSpeed * -initialMovementDir.x, MovementStats.jumpSpeed), ForceMode2D.Impulse);
    }

    public override void ExitState()
    {
        context.SetMoveSpeed(MovementStats.moveSpeed);
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
        //SetSubState(factory.Empty());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        maxJumpTime -= Time.deltaTime;

        context.SetMoveSpeed( Mathf.Lerp(context.CurrentMoveSpeed, MovementStats.moveSpeed, Utils.GetInterpolant(100f)));

        if (context.Controls.Player.Jump.IsPressed() && maxJumpTime > 0)
        {
            context.rb.velocity = new Vector2(MovementStats.moveSpeed * -initialMovementDir.x, MovementStats.jumpSpeed);
        }
        else
        {
            //Debug.Log(context.transform.position.y);
            SwitchState(factory.Fall());
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
