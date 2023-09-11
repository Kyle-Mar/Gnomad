using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerBaseState
{
    public PlayerWallSlideState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        float inputX = context.Controls.Player.Move.ReadValue<Vector2>().x;
        context.CheckIfGrounded();
        if (context.IsGrounded)
        {
            SwitchState(factory.Grounded());
        }
        if (context.Controls.Player.Jump.IsPressed())
        {
            SwitchState(factory.WallJump());
        }
        if (context.IsTouchingWallLeft && inputX >= 0)
        {
            SwitchState(factory.Fall());
        }
        if(context.IsTouchingWallRight && inputX <= 0)
        {
            SwitchState(factory.Fall());
        }
        if (!context.IsTouchingWall)
        {
            SwitchState(factory.Fall());
        }
    }

    public override void EnterState()
    {
        //Debug.Log("slide");

    }


    public override void ExitState()
    {
        context.SetMoveSpeed(MovementStats.moveSpeed);
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        SetSubState(factory.Empty());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Physics2D.gravity = new(0, 0);
        context.rb.velocity = new(0, -3);

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
