using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{ 
    public PlayerGroundedState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
        
    }

    public override void CheckSwitchStates()
    {
        if (context.CanJumpStandard())
        {
            context.ConsumeJumpBuffer();
            SwitchState(context.JumpState);
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

        if (!context.IsGrounded)
        {
            SwitchState(context.FallState);
            return;
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        //context.rb.velocity = new(context.rb.velocity.x, 0);
        Object.Instantiate(context.LandParticles, context.Feet.position, Quaternion.identity);
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
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
        CheckSwitchStates();
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
