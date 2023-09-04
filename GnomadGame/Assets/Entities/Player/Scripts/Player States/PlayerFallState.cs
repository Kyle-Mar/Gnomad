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
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchState(factory.Slide());
        }
        if (Input.GetKeyDown(KeyCode.Q))
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
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        if (Input.GetAxis("Horizontal") != 0)
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
        //TODO: Implement non-frame independent lerp later
        context.rb.velocity = Vector2.Lerp(context.rb.velocity, new Vector2(context.rb.velocity.x, MovementStats.fallSpeed), Time.deltaTime);
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
