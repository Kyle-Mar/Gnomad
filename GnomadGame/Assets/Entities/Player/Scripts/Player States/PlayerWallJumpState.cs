using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gnomad.Utils;
using Unity.VisualScripting.FullSerializer;

public class PlayerWallJumpState : PlayerBaseState
{
    float startJumpTime;
    float jumpTimer;
    float arrestMovementTimer;
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
            return;
        }
        if(jumpTimer <= 0)
        {
            SwitchState(context.FallState);
            return;
        }
        if (context.Controls.Player.Slide.WasPressedThisFrame())
        {
            SwitchState(context.SlideState);
            return;
        }
        if (context.Controls.Player.Dash.WasPressedThisFrame())
        {
            SwitchState(context.DashState);
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
        InitializeSubState();
        arrestMovementTimer = MovementStats.WallJumpArrestMovementTimer;
        startJumpTime = 0;
        initialMovementDir = context.LastMovementDirection;
        //context.FlipComponents();// this is potentially suspect. We may need to do other logic to complete the flip
        // default direction based on which wall you're touching instead?

        jumpTimer = startJumpTime + MovementStats.WallJumpDuration;

        //context.SetMoveSpeed(MovementStats.moveSpeedReduced);
        
        context.rb.AddForce(new Vector2(MovementStats.maxWallJumpHorizontalForce * -initialMovementDir.x,
                            MovementStats.maxWallJumpVerticalForce), ForceMode2D.Impulse);

        Object.Instantiate(context.JumpCloudParticles, context.transform.position, Quaternion.identity);

    }

    public override void ExitState()
    {
        //cSetSubState(context.IdleState);
        //context.SetMoveSpeed(MovementStats.moveSpeed);
    }

    public override void InitializeSubState()
    {
        SetSubState(null);
        /*
        if (context.Controls.Player.Move.ReadValue<Vector2>().x != 0)
        {
            SetSubState(context.RunState);
        }
        else
        {
            SetSubState(context.IdleState);
        }
        */
    }

    public override void UpdateState()
    {
        Debug.Log("Dodingus "+currentSubState);
        if (arrestMovementTimer <= 0 && currentSubState == null)
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
        CheckSwitchStates();

        jumpTimer -= Time.deltaTime;
        arrestMovementTimer -= Time.deltaTime;

        //context.SetMoveSpeed( Mathf.Lerp(context.CurrentMoveSpeed, MovementStats.moveSpeed, Utils.GetInterpolant(100f)));

        ///if (context.Controls.Player.Jump.IsPressed() && jumpTimer > 0)
        context.rb.velocity = new Vector2(MovementStats.moveSpeed * -initialMovementDir.x, MovementStats.jumpSpeed);

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
