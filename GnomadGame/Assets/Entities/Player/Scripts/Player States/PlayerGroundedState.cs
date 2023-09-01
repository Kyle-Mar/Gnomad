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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("TEST");
            SwitchState(factory.Jump());
        }
        context.CheckIfGrounded();
        if (!context.IsGrounded)
        {
            SwitchState(factory.Fall());
        }
    }

    public override void EnterState()
    {
        Debug.Log("HELLO WORLD I AM GROUNDED!");
        //context.rb.velocity = new(context.rb.velocity.x, 0);
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        Debug.Log(Input.GetAxis("Horizontal"));
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
