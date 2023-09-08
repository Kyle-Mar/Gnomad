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
        initialMovementDir = context.LastMovementDirection;
        initialMovementDir.y = 0;
        maxJumpTime = startJumpTime + MovementStats.maxJumpHeight;
        context.rb.velocity = new Vector2(context.rb.velocity.x, MovementStats.jumpSpeed);
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        maxJumpTime -= Time.deltaTime;

        if (context.Controls.Player.Jump.IsPressed() && maxJumpTime > 0)
        {
            context.rb.velocity = (Vector2.up - initialMovementDir) * MovementStats.jumpSpeed;
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
}
