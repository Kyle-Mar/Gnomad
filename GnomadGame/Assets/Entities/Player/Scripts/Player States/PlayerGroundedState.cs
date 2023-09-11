using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        if (context.Controls.Player.Jump.WasPressedThisFrame() || context.JumpBufferTime > 0)
        {
            context.ConsumeJumpBuffer();
            SwitchState(factory.Jump());
        }

        if (context.Controls.Player.Slide.WasPressedThisFrame())
        {
            SwitchState(factory.Slide());
        }

        context.CheckIfGrounded();
        if (!context.IsGrounded)
        {
            SwitchState(factory.Fall());
        }
    }

    public override void EnterState()
    {
        //Debug.Log("HELLO WORLD I AM GROUNDED!");
        //context.rb.velocity = new(context.rb.velocity.x, 0);
        Object.Instantiate(context.land_particles, context.feet.position, Quaternion.identity);

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
