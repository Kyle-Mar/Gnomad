using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        context.CheckIfGrounded();
        if (context.IsGrounded)
        {
            SwitchState(factory.Grounded());
        }

        if (context.DoWallSlide())
        {
            SwitchState(factory.WallSlide());
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
        //Debug.Log("HELLO I AM FALLING");
        //throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        Physics2D.gravity = new(0, -9.8f);
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
        Debug.Log("HELLO WORLD");

        
        Physics2D.gravity = new(0, MovementStats.fallSpeed);

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
